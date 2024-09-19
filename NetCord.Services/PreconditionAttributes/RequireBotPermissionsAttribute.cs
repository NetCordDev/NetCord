using System.Diagnostics.CodeAnalysis;
using System.Text;

using NetCord.Services.ComponentInteractions;

namespace NetCord.Services;

public class RequireBotPermissionsAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IGuildContext, IChannelContext, IGatewayClientContext
{
    public Permissions ChannelPermissions { get; }
    public string? ChannelPermissionsFormat => _channelPermissionsFormat?.Format;

    public Permissions GuildPermissions { get; }
    public string? GuildPermissionsFormat => _guildPermissionsFormat?.Format;

    private readonly CompositeFormat? _channelPermissionsFormat;
    private readonly CompositeFormat? _guildPermissionsFormat;
    private readonly PermissionsType _permissionsType;

    public RequireBotPermissionsAttribute(Permissions channelPermissions,
                                          [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string channelPermissionsFormat = "Required bot permissions: {0}.")
    {
        if (channelPermissions == default)
            return;

        ChannelPermissions = channelPermissions;
        _channelPermissionsFormat = CompositeFormat.Parse(channelPermissionsFormat);
        _permissionsType |= PermissionsType.Channel;
    }

    public RequireBotPermissionsAttribute(Permissions channelPermissions,
                                          Permissions guildPermissions,
                                          [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string channelPermissionsFormat = "Required bot permissions: {0}.",
                                          [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string guildPermissionsFormat = "Required bot guild permissions: {0}.") : this(channelPermissions, channelPermissionsFormat)
    {
        if (guildPermissions == default)
            return;

        GuildPermissions = guildPermissions;
        _guildPermissionsFormat = CompositeFormat.Parse(guildPermissionsFormat);
        _permissionsType |= PermissionsType.Guild;
    }

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        if (context is IComponentInteractionContext interactionContext)
        {
            if (interactionContext.Interaction.Context is not InteractionContextType.Guild)
                return new(PreconditionResult.Fail("The current guild could not be found."));

            var interactionPermissions = interactionContext.Interaction.AppPermissions;
            if (interactionPermissions.HasFlag(Permissions.Administrator))
                return new(PreconditionResult.Success);

            if (_permissionsType.HasFlag(PermissionsType.Channel) && !interactionPermissions.HasFlag(ChannelPermissions))
            {
                var missingPermissions = ChannelPermissions & ~interactionPermissions;
                return new(new MissingPermissionsResult(string.Format(null, _channelPermissionsFormat!, missingPermissions), missingPermissions, MissingPermissionsResultEntityType.Bot, MissingPermissionsResultPermissionType.Channel));
            }

            if (!_permissionsType.HasFlag(PermissionsType.Guild))
                return new(PreconditionResult.Success);

            var guild = context.Guild;
            if (guild is null)
                return new(PreconditionResult.Fail("The current guild could not be found."));

            var botId = context.Client.Cache.User!.Id;

            if (guild.OwnerId == botId)
                return new(PreconditionResult.Success);

            if (!guild.Users.TryGetValue(botId, out var user))
                return new(PreconditionResult.Fail("The bot user could not be found."));

            var permissions = user.GetPermissions(guild);
            if (!permissions.HasFlag(GuildPermissions))
            {
                var missingPermissions = GuildPermissions & ~permissions;
                return new(new MissingPermissionsResult(string.Format(null, _guildPermissionsFormat!, missingPermissions), missingPermissions, MissingPermissionsResultEntityType.Bot, MissingPermissionsResultPermissionType.Guild));
            }

            return new(PreconditionResult.Success);
        }
        else
        {
            var guild = context.Guild;
            if (guild is null)
                return new(PreconditionResult.Fail("The current guild could not be found."));

            var botId = context.Client.Cache.User!.Id;

            if (guild.OwnerId == botId)
                return new(PreconditionResult.Success);

            if (!guild.Users.TryGetValue(botId, out var user))
                return new(PreconditionResult.Fail("The bot user could not be found."));

            var permissions = user.GetPermissions(guild);
            if (_permissionsType.HasFlag(PermissionsType.Guild) && !permissions.HasFlag(GuildPermissions))
            {
                var missingPermissions = GuildPermissions & ~permissions;
                return new(new MissingPermissionsResult(string.Format(null, _guildPermissionsFormat!, missingPermissions), missingPermissions, MissingPermissionsResultEntityType.Bot, MissingPermissionsResultPermissionType.Guild));
            }

            if (!_permissionsType.HasFlag(PermissionsType.Channel))
                return new(PreconditionResult.Success);

            var channel = context.Channel;
            if (channel is null)
                return new(PreconditionResult.Fail("The current channel could not be found."));

            permissions = user.GetChannelPermissions(permissions, (IGuildChannel)channel);
            if (!permissions.HasFlag(ChannelPermissions))
            {
                var missingPermissions = ChannelPermissions & ~permissions;
                return new(new MissingPermissionsResult(string.Format(null, _channelPermissionsFormat!, missingPermissions), missingPermissions, MissingPermissionsResultEntityType.Bot, MissingPermissionsResultPermissionType.Channel));
            }

            return new(PreconditionResult.Success);
        }
    }
}
