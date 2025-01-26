using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class BaseAutocompleteInteractionContext(AutocompleteInteraction interaction) : IAutocompleteInteractionContext
{
    public AutocompleteInteraction Interaction => interaction;
}

public class AutocompleteInteractionContext(AutocompleteInteraction interaction, GatewayClient client)
    : BaseAutocompleteInteractionContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

public class HttpAutocompleteInteractionContext(AutocompleteInteraction interaction, RestClient client)
    : BaseAutocompleteInteractionContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}
