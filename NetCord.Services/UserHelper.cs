namespace NetCord.Services;

internal static class UserHelper
{
    public static void CalculatePermissions(GuildUser user, Guild guild, Permission everyonePermissions, IReadOnlyDictionary<DiscordId, PermissionOverwrite> permissionOverwrites, out Permission permissions, out Permission channelPermissions, out bool administrator)
    {
        if (user == guild.OwnerId)
        {
            permissions = default;
            channelPermissions = default;
            administrator = true;
            return;
        }
        permissions = everyonePermissions;

        foreach (var role in user.GetRoles(guild))
            permissions |= role.Permissions;

        administrator = permissions.HasFlag(Permission.Administrator);
        if (!administrator)
        {
            Permission denied = default;
            Permission allowed = default;
            foreach (var r in user.RolesIds)
            {
                if (permissionOverwrites.TryGetValue(r, out var permission))
                {
                    denied |= permission.Denied;
                    allowed |= permission.Allowed;
                }
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

    public static void EnsureUserHasPermissions(Permission requiredUserPermissions, Permission requiredUserChannelPermissions, Permission userPermissions, Permission userChannelPermissions, bool userAdministrator)
    {
        if (!userAdministrator)
        {
            if (!userPermissions.HasFlag(requiredUserPermissions))
            {
                var missingPermissions = requiredUserPermissions & ~userPermissions;
                throw new PermissionException("Required user permissions: " + missingPermissions, missingPermissions);
            }
            if (!userChannelPermissions.HasFlag(requiredUserChannelPermissions))
            {
                var missingPermissions = requiredUserChannelPermissions & ~userChannelPermissions;
                throw new PermissionException("Required user channel permissions: " + missingPermissions, missingPermissions);
            }
        }
    }

    public static void EnsureBotHasPermissions(Permission requiredBotPermissions, Permission requiredBotChannelPermissions, Permission botPermissions, Permission botChannelPermissions, bool botAdministrator)
    {
        if (!botAdministrator)
        {
            if (!botPermissions.HasFlag(requiredBotPermissions))
            {
                var missingPermissions = requiredBotPermissions & ~botPermissions;
                throw new PermissionException("Required bot permissions: " + missingPermissions, missingPermissions);
            }
            if (!botChannelPermissions.HasFlag(requiredBotChannelPermissions))
            {
                var missingPermissions = requiredBotChannelPermissions & ~botChannelPermissions;
                throw new PermissionException("Required bot channel permissions: " + missingPermissions, missingPermissions);
            }
        }
    }
}