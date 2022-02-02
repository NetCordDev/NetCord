namespace NetCord;

public static class UserHelper
{
    public static void CalculatePermissions(GuildUser user, Guild guild, Permission everyonePermissions, IReadOnlyDictionary<DiscordId, PermissionOverwrite> permissionOverwrites, IEnumerable<GuildRole> guildRoles, out Permission permissions, out Permission channelPermissions, out bool administrator)
    {
        if (user == guild.OwnerId)
        {
            permissions = default;
            channelPermissions = default;
            administrator = true;
            return;
        }
        permissions = everyonePermissions;
        foreach (var role in guildRoles)
            if (user.RolesIds.Contains(role))
                permissions |= role.Permissions;

        administrator = permissions.HasFlag(Permission.Administrator);
        if (!administrator)
        {
            Permission denied = default;
            Permission allowed = default;
            foreach (var r in user.RolesIds)
                if (permissionOverwrites.TryGetValue(r, out var permission))
                {
                    denied |= permission.Denied;
                    allowed |= permission.Allowed;
                }
            if (permissionOverwrites.TryGetValue(user.Id, out var p))
            {
                denied |= p.Denied;
                allowed |= p.Allowed;
            }
            channelPermissions = permissions & ~denied | allowed;
        }
        else
            channelPermissions = default;
    }
}