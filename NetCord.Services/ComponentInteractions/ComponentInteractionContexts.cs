using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.ComponentInteractions;

/// <summary>
/// Base context for handling component interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseComponentInteractionContext(ComponentInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public ComponentInteraction Interaction => interaction;
}

/// <summary>
/// Context for handling component interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseComponentInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class ComponentInteractionContext(ComponentInteraction interaction, GatewayClient client)
    : BaseComponentInteractionContext(interaction),
      IGatewayClientContext,
      IUserContext,
      IGuildContext,
      IChannelContext
{
    public GatewayClient Client => client;

    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based component interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseComponentInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpComponentInteractionContext(ComponentInteraction interaction, RestClient client)
    : BaseComponentInteractionContext(interaction),
      IRestClientContext,
      IUserContext,
      IChannelContext
{
    public RestClient Client => client;

    public User User => Interaction.User;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;
}

/// <summary>
/// Base context for handling message component interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseMessageComponentInteractionContext(MessageComponentInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public MessageComponentInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling message component interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseMessageComponentInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class MessageComponentInteractionContext(MessageComponentInteraction interaction, GatewayClient client)
    : BaseMessageComponentInteractionContext(interaction),
      IGatewayClientContext,
      IRestMessageContext,
      IUserContext,
      IGuildContext,
      IChannelContext
{
    public GatewayClient Client => client;

    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based message component interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseMessageComponentInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpMessageComponentInteractionContext(MessageComponentInteraction interaction, RestClient client)
    : BaseMessageComponentInteractionContext(interaction),
      IRestClientContext,
      IRestMessageContext,
      IUserContext,
      IChannelContext
{
    public RestClient Client => client;

    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;
}

/// <summary>
/// Base context for handling button interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseButtonInteractionContext(ButtonInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public ButtonInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling button interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseButtonInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class ButtonInteractionContext(ButtonInteraction interaction, GatewayClient client)
    : BaseButtonInteractionContext(interaction),
      IGatewayClientContext,
      IRestMessageContext,
      IUserContext,
      IGuildContext,
      IChannelContext
{
    public GatewayClient Client => client;

    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based button interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseButtonInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpButtonInteractionContext(ButtonInteraction interaction, RestClient client)
    : BaseButtonInteractionContext(interaction),
      IRestClientContext,
      IRestMessageContext,
      IUserContext,
      IChannelContext
{
    public RestClient Client => client;

    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;
}

/// <summary>
/// Base context for handling string menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseStringMenuInteractionContext(StringMenuInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public StringMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling string menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseStringMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class StringMenuInteractionContext(StringMenuInteraction interaction, GatewayClient client)
    : BaseStringMenuInteractionContext(interaction),
      IGatewayClientContext,
      IRestMessageContext,
      IUserContext,
      IGuildContext,
      IChannelContext
{
    public GatewayClient Client => client;

    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    /// <summary>
    /// The selected string values from the menu.
    /// </summary>
    public IReadOnlyList<string> SelectedValues => Interaction.Data.SelectedValues;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based string menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseStringMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpStringMenuInteractionContext(StringMenuInteraction interaction, RestClient client)
    : BaseStringMenuInteractionContext(interaction),
      IRestClientContext,
      IRestMessageContext,
      IUserContext,
      IChannelContext
{
    public RestClient Client => client;

    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    /// <summary>
    /// The selected string values from the menu.
    /// </summary>
    public IReadOnlyList<string> SelectedValues => Interaction.Data.SelectedValues;
}
