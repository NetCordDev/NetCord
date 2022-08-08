using NetCord.Services.Interactions;

namespace NetCord.Services;

public class InteractionRequireBotChannelPermissionAttribute<TContext> : PreconditionAttribute<TContext> where TContext : InteractionContext
{
    public Permission ChannelPermission { get; }
    public string Format { get; }

    /// <param name="channelPermission"></param>
    /// <param name="format">{0} - missing permissions</param>
    public InteractionRequireBotChannelPermissionAttribute(Permission channelPermission, string format = "Required bot channel permissions: {0}.")
    {
        ChannelPermission = channelPermission;
        Format = format;
    }

    public override ValueTask EnsureCanExecuteAsync(TContext context)
    {
        if (context.Interaction.AppPermissions.HasValue)
        {
            var permissions = context.Interaction.AppPermissions.GetValueOrDefault();
            if (!permissions.HasFlag(ChannelPermission))
            {
                var missingPermissions = ChannelPermission & ~permissions;
                throw new PermissionException(string.Format(Format, missingPermissions), missingPermissions, PermissionExceptionEntityType.User, PermissionExceptionPermissionType.Channel);
            }
        }
        return default;
    }
}