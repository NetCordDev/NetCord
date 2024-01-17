using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

public static class PartialGuildUserExtensions
{
    public static IEnumerable<Role> GetRoles(this PartialGuildUser user, RestGuild guild)
    {
        var roles = guild.Roles;
        return user.RoleIds.Select(r => roles[r]);
    }

    public static Permissions GetPermissions(this PartialGuildUser user, RestGuild guild)
    {
        if (user.Id == guild.OwnerId)
            return (Permissions)ulong.MaxValue;

        var permissions = guild.EveryoneRole.Permissions;

        foreach (var role in user.GetRoles(guild))
            permissions |= role.Permissions;

        if (permissions.HasFlag(Permissions.Administrator))
            return (Permissions)ulong.MaxValue;

        return permissions;
    }

    public static Permissions GetChannelPermissions(this PartialGuildUser user, RestGuild guild, IGuildChannel channel)
    {
        var guildPermissions = GetPermissions(user, guild);
        return user.GetChannelPermissions(guildPermissions, channel);
    }

    public static Permissions GetChannelPermissions(this PartialGuildUser user, Guild guild, ulong channelId)
    {
        var guildPermissions = GetPermissions(user, guild);
        if (guildPermissions.HasFlag(Permissions.Administrator))
            return (Permissions)ulong.MaxValue;

        return user.GetChannelPermissionsCore(guildPermissions, guild.Channels[channelId]);
    }

    public static Permissions GetChannelPermissions(this PartialGuildUser user, Permissions guildPermissions, IGuildChannel channel)
    {
        if (guildPermissions.HasFlag(Permissions.Administrator))
            return (Permissions)ulong.MaxValue;

        return user.GetChannelPermissionsCore(guildPermissions, channel);
    }

    private static Permissions GetChannelPermissionsCore(this PartialGuildUser user, Permissions guildPermissions, IGuildChannel channel)
    {
        var permissions = guildPermissions;

        var roleIds = user.RoleIds;
        var roleCount = roleIds.Count;

        var overwrites = channel.PermissionOverwrites;

        if (overwrites.TryGetValue(channel.GuildId, out var everyoneOverwrite))
            permissions = (permissions & ~everyoneOverwrite.Denied) | everyoneOverwrite.Allowed;

        Permissions allowed = default;
        Permissions denied = default;

        for (int i = 0; i < roleCount; i++)
        {
            var roleId = roleIds[i];
            if (overwrites.TryGetValue(roleId, out var overwrite))
            {
                allowed |= overwrite.Allowed;
                denied |= overwrite.Denied;
            }
        }

        permissions = (permissions & ~denied) | allowed;

        if (overwrites.TryGetValue(user.Id, out var userOverwrite))
            permissions = (permissions & ~userOverwrite.Denied) | userOverwrite.Allowed;

        return permissions;
    }
}
