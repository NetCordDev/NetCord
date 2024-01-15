using System.Diagnostics.CodeAnalysis;

using NetCord.Services.Interactions;

namespace NetCord.Services;

public class RequireBotPermissionsAttribute<TContext> : PreconditionAttribute<TContext> where TContext : IGuildContext, IChannelContext, IGatewayClientContext
{
    public Permissions ChannelPermissions { get; }
    public string? ChannelPermissionsFormat { get; }

    public Permissions GuildPermissions { get; }
    public string? GuildPermissionsFormat { get; }

    private readonly PermissionsType _permissionsType;

    public RequireBotPermissionsAttribute(Permissions channelPermissions, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string channelPermissionsFormat = "Required bot permissions: {0}.")
    {
        if (channelPermissions == default)
            return;

        ChannelPermissions = channelPermissions;
        ChannelPermissionsFormat = channelPermissionsFormat;
        _permissionsType |= PermissionsType.Channel;
    }

    public RequireBotPermissionsAttribute(Permissions channelPermissions, Permissions guildPermissions, [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string channelPermissionsFormat = "Required bot permissions: {0}.", [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string guildPermissionsFormat = "Required bot guild permissions: {0}.") : this(channelPermissions, channelPermissionsFormat)
    {
        if (guildPermissions == default)
            return;

        GuildPermissions = guildPermissions;
        GuildPermissionsFormat = guildPermissionsFormat;
        _permissionsType |= PermissionsType.Guild;
    }

    public override ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        if (context is IInteractionContext interactionContext)
        {
            var interactionPermissions = interactionContext.Interaction.AppPermissions;

            if (!interactionPermissions.HasValue)
                return new(PreconditionResult.Fail("The current guild could not be found."));

            var interactionPermissionsValue = interactionPermissions.GetValueOrDefault();

            if (interactionPermissionsValue.HasFlag(Permissions.Administrator))
                return new(PreconditionResult.Success);

            if (_permissionsType.HasFlag(PermissionsType.Channel) && !interactionPermissionsValue.HasFlag(ChannelPermissions))
            {
                var missingPermissions = ChannelPermissions & ~interactionPermissions.GetValueOrDefault();
                return new(new MissingPermissionsResult(string.Format(ChannelPermissionsFormat!, missingPermissions), missingPermissions, MissingPermissionsResultEntityType.Bot, MissingPermissionsResultPermissionType.Channel));
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
                return new(new MissingPermissionsResult(string.Format(GuildPermissionsFormat!, missingPermissions), missingPermissions, MissingPermissionsResultEntityType.Bot, MissingPermissionsResultPermissionType.Guild));
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
                return new(new MissingPermissionsResult(string.Format(GuildPermissionsFormat!, missingPermissions), missingPermissions, MissingPermissionsResultEntityType.Bot, MissingPermissionsResultPermissionType.Guild));
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
                return new(new MissingPermissionsResult(string.Format(ChannelPermissionsFormat!, missingPermissions), missingPermissions, MissingPermissionsResultEntityType.Bot, MissingPermissionsResultPermissionType.Channel));
            }

            return new(PreconditionResult.Success);
        }
    }
}
