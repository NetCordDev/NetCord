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

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        if (context.User is GuildInteractionUser guildUser && !guildUser.Permissions.HasFlag(ChannelPermissions))
        {
            var missingPermissions = ChannelPermissions & ~guildUser.Permissions;
            return new(new MissingPermissionsResult(string.Format(Format, missingPermissions), missingPermissions, PermissionsExceptionEntityType.User, PermissionsExceptionPermissionType.Channel));
        }
        return new(PreconditionResult.Success);
    }
}
