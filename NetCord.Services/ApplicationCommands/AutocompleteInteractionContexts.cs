using NetCord.Gateway;
using NetCord.Services.Interactions;

namespace NetCord.Services.ApplicationCommands;

public class BaseAutocompleteInteractionContext : InteractionContext, IAutocompleteInteractionContext
{
    public BaseAutocompleteInteractionContext(ApplicationCommandAutocompleteInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override ApplicationCommandAutocompleteInteraction Interaction { get; }
}

public class AutocompleteInteractionContext : BaseAutocompleteInteractionContext, IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public AutocompleteInteractionContext(ApplicationCommandAutocompleteInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;
    }

    public GatewayClient Client { get; }
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}
