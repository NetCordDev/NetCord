using NetCord.Gateway;

namespace NetCord.Services;

/// <summary>
/// Efficiently checks user channel permissions.
/// </summary>
public class InteractionRequireUserChannelPermissionsAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IUserContext
{
    public Permissions ChannelPermissions { get; }
    public string Format { get; }

    /// <param name="channelPermissions"></param>
    /// <param name="format">{0} - missing permissions</param>
    public InteractionRequireUserChannelPermissionsAttribute(Permissions channelPermissions, string format = "Required user channel permissions: {0}.")
    {
        ChannelPermissions = channelPermissions;
        Format = format;
    }

    public override ValueTask EnsureCanExecuteAsync(TContext context)
    {
        if (context.User is GuildInteractionUser guildUser)
        {
            if (!guildUser.Permissions.HasFlag(ChannelPermissions))
            {
                var missingPermissions = ChannelPermissions & ~guildUser.Permissions;
                throw new PermissionsException(string.Format(Format, missingPermissions), missingPermissions, PermissionsExceptionEntityType.User, PermissionsExceptionPermissionType.Channel);
            }
        }
        return default;
    }
}
