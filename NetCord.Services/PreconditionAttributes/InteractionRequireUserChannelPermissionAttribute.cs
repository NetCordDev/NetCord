using NetCord.Gateway;

namespace NetCord.Services;

/// <summary>
/// Efficiently checks user channel permissions.
/// </summary>
public class InteractionRequireUserChannelPermissionAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IUserContext
{
    public Permission ChannelPermission { get; }
    public string Format { get; }

    /// <param name="channelPermission"></param>
    /// <param name="format">{0} - missing permissions</param>
    public InteractionRequireUserChannelPermissionAttribute(Permission channelPermission, string format = "Required user channel permissions: {0}.")
    {
        ChannelPermission = channelPermission;
        Format = format;
    }

    public override Task EnsureCanExecuteAsync(TContext context)
    {
        if (context.User is GuildInteractionUser guildUser)
        {
            if (!guildUser.Permissions.HasFlag(ChannelPermission))
            {
                var missingPermissions = ChannelPermission & ~guildUser.Permissions;
                throw new PermissionException(string.Format(Format, missingPermissions), missingPermissions, PermissionExceptionEntityType.User, PermissionExceptionPermissionType.Channel);
            }
        }
        return Task.CompletedTask;
    }
}