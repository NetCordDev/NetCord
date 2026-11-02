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
/// Base context for handling string select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseStringSelectInteractionContext(StringSelectInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public StringSelectInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling string select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseStringSelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class StringSelectInteractionContext(StringSelectInteraction interaction, GatewayClient client)
    : BaseStringSelectInteractionContext(interaction),
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
    /// The selected string values from the select.
    /// </summary>
    public IReadOnlyList<string> Values => Interaction.Data.Values;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based string select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseStringSelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpStringSelectInteractionContext(StringSelectInteraction interaction, RestClient client)
    : BaseStringSelectInteractionContext(interaction),
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
    /// The selected string values from the select.
    /// </summary>
    public IReadOnlyList<string> Values => Interaction.Data.Values;
}

/// <summary>
/// Base context for handling entity select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseEntitySelectInteractionContext(EntitySelectInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public EntitySelectInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling entity select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseEntitySelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class EntitySelectInteractionContext(EntitySelectInteraction interaction, GatewayClient client)
    : BaseEntitySelectInteractionContext(interaction),
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
    /// The selected entity IDs from the select.
    /// </summary>
    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.Values;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based entity select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseEntitySelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpEntitySelectInteractionContext(EntitySelectInteraction interaction, RestClient client)
    : BaseEntitySelectInteractionContext(interaction),
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
    /// The selected entity IDs from the select.
    /// </summary>
    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.Values;
}

/// <summary>
/// Base context for handling user select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseUserSelectInteractionContext(UserSelectInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public UserSelectInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling user select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseUserSelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class UserSelectInteractionContext(UserSelectInteraction interaction, GatewayClient client)
    : BaseUserSelectInteractionContext(interaction),
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
    /// The selected users from the select.
    /// </summary>
    public IReadOnlyList<User> SelectedValues => Interaction.Data.Values;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based user select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseUserSelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpUserSelectInteractionContext(UserSelectInteraction interaction, RestClient client)
    : BaseUserSelectInteractionContext(interaction),
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
    /// The selected users from the select.
    /// </summary>
    public IReadOnlyList<User> SelectedValues => Interaction.Data.Values;
}

/// <summary>
/// Base context for handling role select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseRoleSelectInteractionContext(RoleSelectInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public RoleSelectInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling role select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseRoleSelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class RoleSelectInteractionContext(RoleSelectInteraction interaction, GatewayClient client)
    : BaseRoleSelectInteractionContext(interaction),
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
    /// The selected roles from the select.
    /// </summary>
    public IReadOnlyList<Role> SelectedValues => Interaction.Data.Values;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based role select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseRoleSelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpRoleSelectInteractionContext(RoleSelectInteraction interaction, RestClient client)
    : BaseRoleSelectInteractionContext(interaction),
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
    /// The selected roles from the select.
    /// </summary>
    public IReadOnlyList<Role> SelectedValues => Interaction.Data.Values;
}

/// <summary>
/// Base context for handling mentionable select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseMentionableSelectInteractionContext(MentionableSelectInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public MentionableSelectInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling mentionable select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseMentionableSelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class MentionableSelectInteractionContext(MentionableSelectInteraction interaction, GatewayClient client)
    : BaseMentionableSelectInteractionContext(interaction),
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
    /// The selected mentionables (users or roles) from the select.
    /// </summary>
    public IReadOnlyList<Mentionable> SelectedValues => Interaction.Data.Values;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based mentionable select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseMentionableSelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpMentionableSelectInteractionContext(MentionableSelectInteraction interaction, RestClient client)
    : BaseMentionableSelectInteractionContext(interaction),
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
    /// The selected mentionables (users or roles) from the select.
    /// </summary>
    public IReadOnlyList<Mentionable> SelectedValues => Interaction.Data.Values;
}

/// <summary>
/// Base context for handling channel select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseChannelSelectInteractionContext(ChannelSelectInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public ChannelSelectInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling channel select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseChannelSelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class ChannelSelectInteractionContext(ChannelSelectInteraction interaction, GatewayClient client)
    : BaseChannelSelectInteractionContext(interaction),
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
    /// The selected channels from the select.
    /// </summary>
    public IReadOnlyList<Channel> SelectedValues => Interaction.Data.Values;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based channel select interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseChannelSelectInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpChannelSelectInteractionContext(ChannelSelectInteraction interaction, RestClient client)
    : BaseChannelSelectInteractionContext(interaction),
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
    /// The selected channels from the select.
    /// </summary>
    public IReadOnlyList<Channel> SelectedValues => Interaction.Data.Values;
}

/// <summary>
/// Base context for handling modal interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseModalInteractionContext(ModalSubmitInteraction interaction) : IComponentInteractionContext
{
    /// <inheritdoc cref="IComponentInteractionContext.Interaction" />
    public ModalSubmitInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

/// <summary>
/// Context for handling modal interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseModalInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class ModalInteractionContext(ModalSubmitInteraction interaction, GatewayClient client)
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
    public IReadOnlyList<IModalChildComponentData> Components => Interaction.Data.Components;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based modal interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseModalInteractionContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpModalInteractionContext(ModalSubmitInteraction interaction, RestClient client)
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
    public IReadOnlyList<IModalChildComponentData> Components => Interaction.Data.Components;
}
