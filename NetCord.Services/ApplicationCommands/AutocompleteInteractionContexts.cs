using NetCord.Gateway;
using NetCord.Services.Interactions;

namespace NetCord.Services.ApplicationCommands;

public class BaseAutocompleteInteractionContext(AutocompleteInteraction interaction) : InteractionContext(interaction), IAutocompleteInteractionContext
{
    public override AutocompleteInteraction Interaction { get; } = interaction;
}

public class AutocompleteInteractionContext(AutocompleteInteraction interaction, GatewayClient client) : BaseAutocompleteInteractionContext(interaction), IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public GatewayClient Client { get; } = client;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}
