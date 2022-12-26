namespace NetCord.Services;

public class RequireBotPermissionsAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IGuildContext, IChannelContext
{
    public Permissions GuildPermissions { get; }
    public Permissions ChannelPermissions { get; }
    public string GuildPermissionsFormat { get; }
    public string? ChannelPermissionsFormat { get; }

    /// <param name="guildPermissions"></param>
    /// <param name="guildPermissionsFormat">{0} - missing guild permissions</param>
    public RequireBotPermissionsAttribute(Permissions guildPermissions, string guildPermissionsFormat = "Required bot permissions: {0}.")
    {
        GuildPermissions = guildPermissions;
        GuildPermissionsFormat = guildPermissionsFormat;
    }

    /// <param name="guildPermissions"></param>
    /// <param name="channelPermissions"></param>
    /// <param name="guildPermissionsFormat">{0} - missing guild permissions</param>
    /// <param name="channelPermissionsFormat">{0} - missing channel permissions</param>
    public RequireBotPermissionsAttribute(Permissions guildPermissions, Permissions channelPermissions, string guildPermissionsFormat = "Required bot permissions: {0}.", string channelPermissionsFormat = "Required bot channel permissions: {0}.") : this(guildPermissions, guildPermissionsFormat)
    {
        ChannelPermissions = channelPermissions;
        ChannelPermissionsFormat = channelPermissionsFormat;
    }

    public override ValueTask EnsureCanExecuteAsync(TContext context)
    {
        var guild = context.Guild;
        if (guild != null && guild.OwnerId != context.Client.User!.Id)
        {
            var guildUser = guild.Users[context.Client.User.Id];
            Permissions permissions = guild.EveryoneRole.Permissions;
            foreach (var role in guildUser.GetRoles(guild))
                permissions |= role.Permissions;
            if (!permissions.HasFlag(Permissions.Administrator))
            {
                if (!permissions.HasFlag(GuildPermissions))
                {
                    var missingPermissions = GuildPermissions & ~permissions;
                    throw new PermissionsException(string.Format(GuildPermissionsFormat, missingPermissions), missingPermissions, PermissionsExceptionEntityType.Bot, PermissionsExceptionPermissionType.Guild);
                }
                if (ChannelPermissions != default)
                {
                    if (context.Channel == null)
                        throw new EntityNotFoundException("Current channel could not be found.");
                    var permissionOverwrites = ((IGuildChannel)context.Channel).PermissionOverwrites;
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
                        throw new PermissionsException(string.Format(ChannelPermissionsFormat!, missingPermissions), missingPermissions, PermissionsExceptionEntityType.Bot, PermissionsExceptionPermissionType.Channel);
                    }
                }
            }
        }
        return default;
    }
}
