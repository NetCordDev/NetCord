using NetCord.Gateway;
using NetCord.Services.Interactions;

namespace NetCord.Services.ApplicationCommands;

public class AutocompleteInteractionContext : InteractionContext, IAutocompleteInteractionContext
{
    public AutocompleteInteractionContext(ApplicationCommandAutocompleteInteraction interaction, GatewayClient client) : base(client)
    {
        Interaction = interaction;
    }

    public override ApplicationCommandAutocompleteInteraction Interaction { get; }
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}
