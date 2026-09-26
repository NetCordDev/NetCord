using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

/// <summary>
/// Base context for handling autocomplete interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseAutocompleteInteractionContext(AutocompleteInteraction interaction) : IAutocompleteInteractionContext
{
    public AutocompleteInteraction Interaction => interaction;
}

/// <summary>
/// Context for handling autocomplete interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseAutocompleteInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class AutocompleteInteractionContext(AutocompleteInteraction interaction, GatewayClient client)
    : BaseAutocompleteInteractionContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;
    public Guild? Guild => Interaction.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based autocomplete interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseAutocompleteInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpAutocompleteInteractionContext(AutocompleteInteraction interaction, RestClient client)
    : BaseAutocompleteInteractionContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;
}
