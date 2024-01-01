namespace NetCord.Services;

public class RequireUserPermissionsAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IUserContext, IGuildContext, IChannelContext
{
    public Permissions GuildPermissions { get; }
    public Permissions ChannelPermissions { get; }
    public string GuildPermissionsFormat { get; }
    public string? ChannelPermissionsFormat { get; }

    /// <param name="guildPermissions"></param>
    /// <param name="guildPermissionsFormat">{0} - missing guild permissions</param>
    public RequireUserPermissionsAttribute(Permissions guildPermissions, string guildPermissionsFormat = "Required user permissions: {0}.")
    {
        GuildPermissions = guildPermissions;
        GuildPermissionsFormat = guildPermissionsFormat;
    }

    /// <param name="guildPermissions"></param>
    /// <param name="channelPermissions"></param>
    /// <param name="guildPermissionsFormat">{0} - missing guild permissions</param>
    /// <param name="channelPermissionsFormat">{0} - missing channel permissions</param>
    public RequireUserPermissionsAttribute(Permissions guildPermissions, Permissions channelPermissions, string guildPermissionsFormat = "Required user permissions: {0}.", string channelPermissionsFormat = "Required user channel permissions: {0}.") : this(guildPermissions, guildPermissionsFormat)
    {
        ChannelPermissions = channelPermissions;
        ChannelPermissionsFormat = channelPermissionsFormat;
    }

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        var guild = context.Guild;
        if (guild is not null && guild.OwnerId != context.User.Id)
        {
            var guildUser = (GuildUser)context.User;
            var permissions = guild.EveryoneRole.Permissions;
            foreach (var role in guildUser.GetRoles(guild))
                permissions |= role.Permissions;
            if (!permissions.HasFlag(Permissions.Administrator))
            {
                if (!permissions.HasFlag(GuildPermissions))
                {
                    var missingPermissions = GuildPermissions & ~permissions;
                    return new(new MissingPermissionsResult(string.Format(GuildPermissionsFormat, missingPermissions), missingPermissions, PermissionsExceptionEntityType.User, PermissionsExceptionPermissionType.Guild));
                }
                if (ChannelPermissions != default)
                {
                    var channel = context.Channel;
                    if (channel is null)
                        return new(PreconditionResult.Fail("The current channel could not be found."));
                    var permissionOverwrites = ((IGuildChannel)channel).PermissionOverwrites;
                    Permissions denied = default;
                    Permissions allowed = default;
                    foreach (var r in guildUser.RoleIds)
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
                    if (!permissions.HasFlag(ChannelPermissions))
                    {
                        var missingPermissions = ChannelPermissions & ~permissions;
                        return new(new MissingPermissionsResult(string.Format(ChannelPermissionsFormat!, missingPermissions), missingPermissions, PermissionsExceptionEntityType.User, PermissionsExceptionPermissionType.Channel));
                    }
                }
            }
        }

        return new(PreconditionResult.Success);
    }
}
