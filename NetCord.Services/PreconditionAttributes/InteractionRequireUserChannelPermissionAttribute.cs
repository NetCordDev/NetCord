namespace NetCord.Services;

/// <summary>
/// Efficiently checks user channel permissions.
/// </summary>
public class InteractionRequireUserChannelPermissionAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IUserContext
{
    public Permission ChannelPermission { get; }

    public InteractionRequireUserChannelPermissionAttribute(Permission channelPermission)
    {
        ChannelPermission = channelPermission;
    }

    public override Task EnsureCanExecuteAsync(TContext context)
    {
        if (context.User is GuildInteractionUser guildUser)
        {
            if (!guildUser.Permissions.HasFlag(ChannelPermission))
            {
                var missingPermissions = ChannelPermission & ~guildUser.Permissions;
                throw new PermissionException("Required user channel permissions: " + missingPermissions, missingPermissions, PermissionExceptionEntityType.User, PermissionExceptionPermissionType.Channel);
            }
        }
        return Task.CompletedTask;
    }
}
