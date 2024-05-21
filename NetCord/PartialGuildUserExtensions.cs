using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Contains methods providing additional functionality for <see cref="PartialGuildUser"/>.
/// </summary>
public static class PartialGuildUserExtensions
{
    /// <summary>
    /// Returns an <see cref="IEnumerable{Role}"/> object belonging to the <see cref="PartialGuildUser"/> by acquiring it from the specificied <see cref="RestGuild"/>.
    /// </summary>
    /// <param name="user"> The <see cref="PartialGuildUser"/> to acquire roles for. </param>
    /// <param name="guild"> The <see cref="RestGuild"/> to acquire the roles from. </param>
    public static IEnumerable<Role> GetRoles(this PartialGuildUser user, RestGuild guild)
    {
        var roles = guild.Roles;
        return user.RoleIds.Select(r => roles[r]);
    }

    /// <summary>
    /// Returns a <see cref="Permissions"/> object belonging to the <see cref="PartialGuildUser"/> by acquiring it from the specificied <see cref="RestGuild"/>.
    /// </summary>
    /// <param name="user"> The <see cref="PartialGuildUser"/> to acquire permissions for. </param>
    /// <param name="guild"> The <see cref="RestGuild"/> to acquire the permissions from. </param>
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

    /// <summary>
    /// Returns a <see cref="IGuildChannel"/>-specific <see cref="Permissions"/> object belonging to the <see cref="PartialGuildUser"/> by acquiring it from the specificied <see cref="RestGuild"/>.
    /// </summary>
    /// <param name="user"> The <see cref="PartialGuildUser"/> to acquire permissions for. </param>
    /// <param name="guild"> The <see cref="RestGuild"/> to acquire the permissions from. </param>
    /// <param name="channel"> The <see cref="IGuildChannel"/> to acquire the permissions for. </param>
    public static Permissions GetChannelPermissions(this PartialGuildUser user, RestGuild guild, IGuildChannel channel)
    {
        var guildPermissions = GetPermissions(user, guild);
        return user.GetChannelPermissions(guildPermissions, channel);
    }

    /// <summary>
    /// Returns a <see cref="IGuildChannel"/>-specific <see cref="Permissions"/> object belonging to the <see cref="PartialGuildUser"/> by acquiring it from the specificied <see cref="RestGuild"/>.
    /// </summary>
    /// <param name="user"> The <see cref="PartialGuildUser"/> to acquire permissions for. </param>
    /// <param name="guild"> The <see cref="RestGuild"/> to acquire the permissions from. </param>
    /// <param name="channelId"> The ID of the <see cref="IGuildChannel"/> to acquire the permissions for. </param>
    public static Permissions GetChannelPermissions(this PartialGuildUser user, Guild guild, ulong channelId)
    {
        var guildPermissions = GetPermissions(user, guild);
        if (guildPermissions.HasFlag(Permissions.Administrator))
            return (Permissions)ulong.MaxValue;

        return user.GetChannelPermissionsCore(guildPermissions, guild.Channels[channelId]);
    }

    /// <summary>
    /// Returns a <see cref="IGuildChannel"/>-specific <see cref="Permissions"/> object belonging to the <see cref="PartialGuildUser"/> by acquiring it from the specificied <paramref name="guildPermissions"/>.
    /// </summary>
    /// <param name="user"> The <see cref="PartialGuildUser"/> to acquire permissions for. </param>
    /// <param name="guildPermissions"> The <see cref="Permissions"/> object to acquire permissions from. </param>
    /// <param name="channel"> The <see cref="IGuildChannel"/> to acquire the permissions for. </param>
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
