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

/// <summary>
/// Base context for handling entity menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseEntityMenuInteractionContext(EntityMenuInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public EntityMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling entity menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseEntityMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class EntityMenuInteractionContext(EntityMenuInteraction interaction, GatewayClient client)
    : BaseEntityMenuInteractionContext(interaction),
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
    /// The selected entity IDs from the menu.
    /// </summary>
    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.SelectedValues;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based entity menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseEntityMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpEntityMenuInteractionContext(EntityMenuInteraction interaction, RestClient client)
    : BaseEntityMenuInteractionContext(interaction),
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
    /// The selected entity IDs from the menu.
    /// </summary>
    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.SelectedValues;
}

/// <summary>
/// Base context for handling user menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseUserMenuInteractionContext(UserMenuInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public UserMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling user menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseUserMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class UserMenuInteractionContext(UserMenuInteraction interaction, GatewayClient client)
    : BaseUserMenuInteractionContext(interaction),
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
    /// The selected users from the menu.
    /// </summary>
    public IReadOnlyList<User> SelectedValues => Interaction.Data.SelectedValues;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based user menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseUserMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpUserMenuInteractionContext(UserMenuInteraction interaction, RestClient client)
    : BaseUserMenuInteractionContext(interaction),
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
    /// The selected users from the menu.
    /// </summary>
    public IReadOnlyList<User> SelectedValues => Interaction.Data.SelectedValues;
}

/// <summary>
/// Base context for handling role menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseRoleMenuInteractionContext(RoleMenuInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public RoleMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling role menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseRoleMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class RoleMenuInteractionContext(RoleMenuInteraction interaction, GatewayClient client)
    : BaseRoleMenuInteractionContext(interaction),
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
    /// The selected roles from the menu.
    /// </summary>
    public IReadOnlyList<Role> SelectedValues => Interaction.Data.SelectedValues;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based role menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseRoleMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpRoleMenuInteractionContext(RoleMenuInteraction interaction, RestClient client)
    : BaseRoleMenuInteractionContext(interaction),
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
    /// The selected roles from the menu.
    /// </summary>
    public IReadOnlyList<Role> SelectedValues => Interaction.Data.SelectedValues;
}

/// <summary>
/// Base context for handling mentionable menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseMentionableMenuInteractionContext(MentionableMenuInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public MentionableMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling mentionable menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseMentionableMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class MentionableMenuInteractionContext(MentionableMenuInteraction interaction, GatewayClient client)
    : BaseMentionableMenuInteractionContext(interaction),
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
    /// The selected mentionables (users or roles) from the menu.
    /// </summary>
    public IReadOnlyList<Mentionable> SelectedValues => Interaction.Data.SelectedValues;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based mentionable menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseMentionableMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpMentionableMenuInteractionContext(MentionableMenuInteraction interaction, RestClient client)
    : BaseMentionableMenuInteractionContext(interaction),
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
    /// The selected mentionables (users or roles) from the menu.
    /// </summary>
    public IReadOnlyList<Mentionable> SelectedValues => Interaction.Data.SelectedValues;
}

/// <summary>
/// Base context for handling channel menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseChannelMenuInteractionContext(ChannelMenuInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public ChannelMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling channel menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseChannelMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class ChannelMenuInteractionContext(ChannelMenuInteraction interaction, GatewayClient client)
    : BaseChannelMenuInteractionContext(interaction),
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
    /// The selected channels from the menu.
    /// </summary>
    public IReadOnlyList<Channel> SelectedValues => Interaction.Data.SelectedValues;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based channel menu interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseChannelMenuInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpChannelMenuInteractionContext(ChannelMenuInteraction interaction, RestClient client)
    : BaseChannelMenuInteractionContext(interaction),
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
    /// The selected channels from the menu.
    /// </summary>
    public IReadOnlyList<Channel> SelectedValues => Interaction.Data.SelectedValues;
}

/// <summary>
/// Base context for handling modal interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseModalInteractionContext(ModalInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public ModalInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling modal interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseModalInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class ModalInteractionContext(ModalInteraction interaction, GatewayClient client)
    : BaseModalInteractionContext(interaction),
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

    /// <summary>
    /// The components submitted with the modal.
    /// </summary>
    public IReadOnlyList<IModalComponent> Components => Interaction.Data.Components;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based modal interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseModalInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpModalInteractionContext(ModalInteraction interaction, RestClient client)
    : BaseModalInteractionContext(interaction),
      IRestClientContext,
      IUserContext,
      IChannelContext
{
    public RestClient Client => client;

    public User User => Interaction.User;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    /// <summary>
    /// The components submitted with the modal.
    /// </summary>
    public IReadOnlyList<IModalComponent> Components => Interaction.Data.Components;
}
