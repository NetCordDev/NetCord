using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.ComponentInteractions;

public class ComponentInteractionContext(ComponentInteraction interaction) : IComponentInteractionContext
{
    public virtual ComponentInteraction Interaction { get; } = interaction;
}

public class BaseButtonInteractionContext(ButtonInteraction interaction) : ComponentInteractionContext(interaction)
{
    public override ButtonInteraction Interaction { get; } = interaction;
}

public class ButtonInteractionContext(ButtonInteraction interaction, GatewayClient client) : BaseButtonInteractionContext(interaction), IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public GatewayClient Client { get; } = client;
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
}

public class HttpButtonInteractionContext(ButtonInteraction interaction, RestClient client) : BaseButtonInteractionContext(interaction), IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public RestClient Client { get; } = client;
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
}

public class BaseStringMenuInteractionContext(StringMenuInteraction interaction) : ComponentInteractionContext(interaction)
{
    public override StringMenuInteraction Interaction { get; } = interaction;
}

public class StringMenuInteractionContext(StringMenuInteraction interaction, GatewayClient client) : BaseStringMenuInteractionContext(interaction), IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public GatewayClient Client { get; } = client;
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<string> SelectedValues => Interaction.Data.SelectedValues;
}

public class HttpStringMenuInteractionContext(StringMenuInteraction interaction, RestClient client) : BaseStringMenuInteractionContext(interaction), IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public RestClient Client { get; } = client;
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<string> SelectedValues => Interaction.Data.SelectedValues;
}

public class BaseEntityMenuInteractionContext(EntityMenuInteraction interaction) : ComponentInteractionContext(interaction)
{
    public override EntityMenuInteraction Interaction { get; } = interaction;
}

public class EntityMenuInteractionContext(EntityMenuInteraction interaction, GatewayClient client) : BaseEntityMenuInteractionContext(interaction), IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public GatewayClient Client { get; } = client;
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.SelectedValues;
}

public class HttpEntityMenuInteractionContext(EntityMenuInteraction interaction, RestClient client) : BaseEntityMenuInteractionContext(interaction), IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public RestClient Client { get; } = client;
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.SelectedValues;
}

public class BaseUserMenuInteractionContext(UserMenuInteraction interaction) : BaseEntityMenuInteractionContext(interaction)
{
    public override UserMenuInteraction Interaction { get; } = interaction;
}

public class UserMenuInteractionContext : BaseUserMenuInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public UserMenuInteractionContext(UserMenuInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedUsers = [];
        else
        {
            var users = resolvedData.Users!;
            SelectedUsers = data.SelectedValues.Select(v => users[v]).ToArray();
        }
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<User> SelectedUsers { get; }
}

public class HttpUserMenuInteractionContext : BaseUserMenuInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpUserMenuInteractionContext(UserMenuInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedUsers = [];
        else
        {
            var users = resolvedData.Users!;
            SelectedUsers = data.SelectedValues.Select(v => users[v]).ToArray();
        }
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<User> SelectedUsers { get; }
}

public class BaseRoleMenuInteractionContext(RoleMenuInteraction interaction) : BaseEntityMenuInteractionContext(interaction)
{
    public override RoleMenuInteraction Interaction { get; } = interaction;
}

public class RoleMenuInteractionContext : BaseRoleMenuInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public RoleMenuInteractionContext(RoleMenuInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedRoles = [];
        else
        {
            var roles = resolvedData.Roles!;
            SelectedRoles = data.SelectedValues.Select(v => roles[v]).ToArray();
        }
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Role> SelectedRoles { get; }
}

public class HttpRoleMenuInteractionContext : BaseRoleMenuInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpRoleMenuInteractionContext(RoleMenuInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedRoles = [];
        else
        {
            var roles = resolvedData.Roles!;
            SelectedRoles = data.SelectedValues.Select(v => roles[v]).ToArray();
        }
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Role> SelectedRoles { get; }
}

public class BaseMentionableMenuInteractionContext(MentionableMenuInteraction interaction) : BaseEntityMenuInteractionContext(interaction)
{
    public override MentionableMenuInteraction Interaction { get; } = interaction;
}

public class MentionableMenuInteractionContext : BaseMentionableMenuInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public MentionableMenuInteractionContext(MentionableMenuInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;

        var data = Interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedMentionables = [];
        else
        {
            var users = resolvedData.Users;
            var roles = resolvedData.Roles;
            SelectedMentionables = users is null
                ? roles is null
                    ? []
                    : data.SelectedValues.Select(v => new Mentionable(roles[v])).ToArray()
                : roles is null
                    ? data.SelectedValues.Select(v => new Mentionable(users[v])).ToArray()
                    : data.SelectedValues.Select(v => users.TryGetValue(v, out var user) ? new Mentionable(user) : new Mentionable(roles[v])).ToArray();
        }
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Mentionable> SelectedMentionables { get; }
}

public class HttpMentionableMenuInteractionContext : BaseMentionableMenuInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpMentionableMenuInteractionContext(MentionableMenuInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;

        var data = Interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedMentionables = [];
        else
        {
            var users = resolvedData.Users;
            var roles = resolvedData.Roles;
            SelectedMentionables = users is null
                ? roles is null
                    ? []
                    : data.SelectedValues.Select(v => new Mentionable(roles[v])).ToArray()
                : roles is null
                    ? data.SelectedValues.Select(v => new Mentionable(users[v])).ToArray()
                    : data.SelectedValues.Select(v => users.TryGetValue(v, out var user) ? new Mentionable(user) : new Mentionable(roles[v])).ToArray();
        }
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Mentionable> SelectedMentionables { get; }
}

public class BaseChannelMenuInteractionContext(ChannelMenuInteraction interaction) : BaseEntityMenuInteractionContext(interaction)
{
    public override ChannelMenuInteraction Interaction { get; } = interaction;
}

public class ChannelMenuInteractionContext : BaseChannelMenuInteractionContext, IGatewayClientContext, IRestMessageContext, IUserContext, IGuildContext, IChannelContext
{
    public ChannelMenuInteractionContext(ChannelMenuInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedChannels = [];
        else
        {
            var channels = resolvedData.Channels!;
            SelectedChannels = data.SelectedValues.Select(v => channels[v]).ToArray();
        }
    }

    public GatewayClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Channel> SelectedChannels { get; }
}

public class HttpChannelMenuInteractionContext : BaseChannelMenuInteractionContext, IRestClientContext, IRestMessageContext, IUserContext, IChannelContext
{
    public HttpChannelMenuInteractionContext(ChannelMenuInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;

        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData is null)
            SelectedChannels = [];
        else
        {
            var channels = resolvedData.Channels!;
            SelectedChannels = data.SelectedValues.Select(v => channels[v]).ToArray();
        }
    }

    public RestClient Client { get; }
    public RestMessage Message => Interaction.Message;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<Channel> SelectedChannels { get; }
}

public class BaseModalInteractionContext(ModalInteraction interaction) : ComponentInteractionContext(interaction)
{
    public override ModalInteraction Interaction { get; } = interaction;
}

public class ModalInteractionContext(ModalInteraction interaction, GatewayClient client) : BaseModalInteractionContext(interaction), IGatewayClientContext, IUserContext, IGuildContext, IChannelContext
{
    public GatewayClient Client { get; } = client;
    public User User => Interaction.User;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<TextInput> Components => Interaction.Data.Components;
}

public class HttpModalInteractionContext(ModalInteraction interaction, RestClient client) : BaseModalInteractionContext(interaction), IRestClientContext, IUserContext, IChannelContext
{
    public RestClient Client { get; } = client;
    public User User => Interaction.User;
    public TextChannel Channel => Interaction.Channel;
    public IReadOnlyList<TextInput> Components => Interaction.Data.Components;
}
