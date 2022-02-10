namespace NetCord.Services;

public class RequireUserPermissionAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IUserContext, IGuildContext, IChannelContext
{
    public Permission GuildPermission { get; }

    public Permission ChannelPermission { get; }

    public RequireUserPermissionAttribute(Permission guildPermission)
    {
        GuildPermission = guildPermission;
    }

    public RequireUserPermissionAttribute(Permission generalPermission, Permission channelPermission)
    {
        GuildPermission = generalPermission;
        ChannelPermission = channelPermission;
    }

    public override Task EnsureCanExecuteAsync(TContext context)
    {
        var guild = context.Guild;
        if (guild != null && guild.OwnerId != context.User.Id)
        {
            var guildUser = (GuildUser)context.User;
            Permission permissions = guild.EveryoneRole.Permissions;
            foreach (var role in guildUser.GetRoles(guild))
                permissions |= role.Permissions;
            if (!permissions.HasFlag(Permission.Administrator))
            {
                if (!permissions.HasFlag(GuildPermission))
                {
                    var missingPermissions = GuildPermission & ~permissions;
                    throw new PermissionException("Required user permissions: " + missingPermissions, missingPermissions, PermissionExceptionEntityType.User, PermissionExceptionPermissionType.Guild);
                }
                if (ChannelPermission != default)
                {
                    var permissionOverwrites = ((IGuildChannel)context.Channel).PermissionOverwrites;
                    Permission denied = default;
                    Permission allowed = default;
                    foreach (var r in guildUser.RolesIds)
                    {
                        if (permissionOverwrites.TryGetValue(r, out var permissionOverwrite))
                        {
                            denied |= permissionOverwrite.Denied;
                            allowed |= permissionOverwrite.Allowed;
                        }
                    }
                    if (permissionOverwrites.TryGetValue(guildUser.Id, out var o))
                    {
                        denied |= o.Denied;
                        allowed |= o.Allowed;
                    }
                    permissions = (permissions & ~denied) | allowed;
                    if (!permissions.HasFlag(ChannelPermission))
                    {
                        var missingPermissions = ChannelPermission & ~permissions;
                        throw new PermissionException("Required user channel permissions: " + missingPermissions, missingPermissions, PermissionExceptionEntityType.User, PermissionExceptionPermissionType.Channel);
                    }
                }
            }
        }
        return Task.CompletedTask;
    }
}