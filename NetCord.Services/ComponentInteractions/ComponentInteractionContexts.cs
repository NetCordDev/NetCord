using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.ComponentInteractions;

public class BaseComponentInteractionContext(ComponentInteraction interaction) : IComponentInteractionContext
{
    public ComponentInteraction Interaction => interaction;
}

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
    public TextChannel Channel => Interaction.Channel;
}

public class HttpComponentInteractionContext(ComponentInteraction interaction, RestClient client)
    : BaseComponentInteractionContext(interaction),
      IRestClientContext,
      IUserContext,
      IChannelContext
{
    public RestClient Client => client;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
}

public class BaseMessageComponentInteractionContext(MessageComponentInteraction interaction) : IComponentInteractionContext
{
    public MessageComponentInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

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
    public TextChannel Channel => Interaction.Channel;
}

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
    public TextChannel Channel => Interaction.Channel;
}

public class BaseButtonInteractionContext(ButtonInteraction interaction) : IComponentInteractionContext
{
    public ButtonInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

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
    public TextChannel Channel => Interaction.Channel;
}

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
    public TextChannel Channel => Interaction.Channel;
}

public class BaseStringMenuInteractionContext(StringMenuInteraction interaction) : IComponentInteractionContext
{
    public StringMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<string> SelectedValues => Interaction.Data.SelectedValues;
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<string> SelectedValues => Interaction.Data.SelectedValues;
}

public class BaseEntityMenuInteractionContext(EntityMenuInteraction interaction) : IComponentInteractionContext
{
    public EntityMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.SelectedValues;
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.SelectedValues;
}

public class BaseUserMenuInteractionContext(UserMenuInteraction interaction) : IComponentInteractionContext
{
    public UserMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<User> SelectedUsers { get; } = Utils.GetUserMenuValues(interaction);
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<User> SelectedUsers { get; } = Utils.GetUserMenuValues(interaction);
}

public class BaseRoleMenuInteractionContext(RoleMenuInteraction interaction) : IComponentInteractionContext
{
    public RoleMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Role> SelectedRoles { get; } = Utils.GetRoleMenuValues(interaction);
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Role> SelectedRoles { get; } = Utils.GetRoleMenuValues(interaction);
}

public class BaseMentionableMenuInteractionContext(MentionableMenuInteraction interaction) : IComponentInteractionContext
{
    public MentionableMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Mentionable> SelectedMentionables { get; } = Utils.GetMentionableMenuValues(interaction);
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Mentionable> SelectedMentionables { get; } = Utils.GetMentionableMenuValues(interaction);
}

public class BaseChannelMenuInteractionContext(ChannelMenuInteraction interaction) : IComponentInteractionContext
{
    public ChannelMenuInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Channel> SelectedChannels { get; } = Utils.GetChannelMenuValues(interaction);
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Channel> SelectedChannels { get; } = Utils.GetChannelMenuValues(interaction);
}

public class BaseModalInteractionContext(ModalInteraction interaction) : IComponentInteractionContext
{
    public ModalInteraction Interaction => interaction;

    ComponentInteraction IComponentInteractionContext.Interaction => interaction;
}

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
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<IComponent> Components => Interaction.Data.Components;
}

public class HttpModalInteractionContext(ModalInteraction interaction, RestClient client)
    : BaseModalInteractionContext(interaction),
      IRestClientContext,
      IUserContext,
      IChannelContext
{
    public RestClient Client => client;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<IComponent> Components => Interaction.Data.Components;
}

static file class Utils
{
    public static IReadOnlyList<User> GetUserMenuValues(UserMenuInteraction interaction)
    {
        var data = interaction.Data;
        var resolvedData = data.ResolvedData;

        IReadOnlyList<User> result;

        if (resolvedData is null)
            result = [];
        else
        {
            var users = resolvedData.Users!;
            result = data.SelectedValues.Select(v => users[v]).ToArray();
        }

        return result;
    }

    public static IReadOnlyList<Role> GetRoleMenuValues(RoleMenuInteraction interaction)
    {
        var data = interaction.Data;
        var resolvedData = data.ResolvedData;

        IReadOnlyList<Role> result;

        if (resolvedData is null)
            result = [];
        else
        {
            var roles = resolvedData.Roles!;
            result = data.SelectedValues.Select(v => roles[v]).ToArray();
        }

        return result;
    }

    public static IReadOnlyList<Mentionable> GetMentionableMenuValues(MentionableMenuInteraction interaction)
    {
        var data = interaction.Data;
        var resolvedData = data.ResolvedData;

        IReadOnlyList<Mentionable> result;

        if (resolvedData is null)
            result = [];
        else
        {
            var users = resolvedData.Users;
            var roles = resolvedData.Roles;
            result = users is null
                ? roles is null
                    ? []
                    : data.SelectedValues.Select(v => new Mentionable(roles[v])).ToArray()
                : roles is null
                    ? data.SelectedValues.Select(v => new Mentionable(users[v])).ToArray()
                    : data.SelectedValues.Select(v => users.TryGetValue(v, out var user) ? new Mentionable(user) : new Mentionable(roles[v])).ToArray();
        }

        return result;
    }

    public static IReadOnlyList<Channel> GetChannelMenuValues(ChannelMenuInteraction interaction)
    {
        var data = interaction.Data;
        var resolvedData = data.ResolvedData;

        IReadOnlyList<Channel> result;

        if (resolvedData is null)
            result = [];
        else
        {
            var channels = resolvedData.Channels!;
            result = data.SelectedValues.Select(v => channels[v]).ToArray();
        }

        return result;
    }
}
