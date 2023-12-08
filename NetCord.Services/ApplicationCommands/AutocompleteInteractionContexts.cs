using NetCord.Gateway;
using NetCord.Services.Interactions;

namespace NetCord.Services.ApplicationCommands;

public class BaseAutocompleteInteractionContext : InteractionContext, IAutocompleteInteractionContext
{
    public BaseAutocompleteInteractionContext(AutocompleteInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override AutocompleteInteraction Interaction { get; }
}

public class AutocompleteInteractionContext : BaseAutocompleteInteractionContext, IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public AutocompleteInteractionContext(AutocompleteInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;
    }

    public GatewayClient Client { get; }
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}
