using NetCord.Gateway;

namespace NetCord.Services.ApplicationCommands;

public class BaseAutocompleteInteractionContext(AutocompleteInteraction interaction) : IAutocompleteInteractionContext
{
    public virtual AutocompleteInteraction Interaction { get; } = interaction;
}

public class AutocompleteInteractionContext(AutocompleteInteraction interaction, GatewayClient client) : BaseAutocompleteInteractionContext(interaction), IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public GatewayClient Client { get; } = client;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}
