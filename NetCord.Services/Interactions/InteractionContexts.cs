using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.Interactions;

public abstract class InteractionContext : IContext
{
    protected InteractionContext(GatewayClient client)
    {
        Client = client;
    }

    public abstract Interaction Interaction { get; }

    public GatewayClient Client { get; }
}

public class BaseButtonInteractionContext : InteractionContext
{
    public override ButtonInteraction Interaction { get; }

    public BaseButtonInteractionContext(ButtonInteraction interaction, GatewayClient client) : base(client)
    {
        Interaction = interaction;
    }
}

public class BaseStringMenuInteractionContext : InteractionContext
{
    public override StringMenuInteraction Interaction { get; }

    public BaseStringMenuInteractionContext(StringMenuInteraction interaction, GatewayClient client) : base(client)
    {
        Interaction = interaction;
    }
}

public class BaseEntityMenuInteractionContext : InteractionContext
{
    public override EntityMenuInteraction Interaction { get; }

    public BaseEntityMenuInteractionContext(EntityMenuInteraction interaction, GatewayClient client) : base(client)
    {
        Interaction = interaction;
    }
}

public class BaseUserMenuInteractionContext : BaseEntityMenuInteractionContext
{
    public override UserMenuInteraction Interaction { get; }

    public BaseUserMenuInteractionContext(UserMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
    }
}

public class BaseRoleMenuInteractionContext : BaseEntityMenuInteractionContext
{
    public override RoleMenuInteraction Interaction { get; }

    public BaseRoleMenuInteractionContext(RoleMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
    }
}

public class BaseMentionableMenuInteractionContext : BaseEntityMenuInteractionContext
{
    public override MentionableMenuInteraction Interaction { get; }

    public BaseMentionableMenuInteractionContext(MentionableMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
    }
}

public class BaseChannelMenuInteractionContext : BaseEntityMenuInteractionContext
{
    public override ChannelMenuInteraction Interaction { get; }

    public BaseChannelMenuInteractionContext(ChannelMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
    }
}

public class BaseModalSubmitInteractionContext : InteractionContext
{
    public override ModalSubmitInteraction Interaction { get; }

    public BaseModalSubmitInteractionContext(ModalSubmitInteraction interaction, GatewayClient client) : base(client)
    {
        Interaction = interaction;
    }
}

public class ButtonInteractionContext : BaseButtonInteractionContext, IUserContext, IGuildContext, IChannelContext, IRestMessageContext
{
    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    public TextChannel? Channel => Interaction.Channel;

    public ButtonInteractionContext(ButtonInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}

public class StringMenuInteractionContext : BaseStringMenuInteractionContext, IUserContext, IGuildContext, IChannelContext, IRestMessageContext
{
    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    public TextChannel? Channel => Interaction.Channel;

    public IReadOnlyList<string> SelectedValues => Interaction.Data.SelectedValues;

    public StringMenuInteractionContext(StringMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}

public class EntityMenuInteractionContext : BaseEntityMenuInteractionContext, IUserContext, IGuildContext, IChannelContext, IRestMessageContext
{
    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    public TextChannel? Channel => Interaction.Channel;

    public IReadOnlyList<ulong> SelectedValues => Interaction.Data.SelectedValues;

    public EntityMenuInteractionContext(EntityMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}

public class UserMenuInteractionContext : EntityMenuInteractionContext
{
    public override UserMenuInteraction Interaction { get; }

    public IReadOnlyList<User> SelectedUsers { get; }

    public UserMenuInteractionContext(UserMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData != null)
        {
            var users = resolvedData.Users!;
            SelectedUsers = data.SelectedValues.Select(v => users[v]).ToArray();
        }
        else
            SelectedUsers = Array.Empty<User>();
    }
}

public class RoleMenuInteractionContext : EntityMenuInteractionContext
{
    public override RoleMenuInteraction Interaction { get; }

    public IReadOnlyList<Role> SelectedRoles { get; }

    public RoleMenuInteractionContext(RoleMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData != null)
        {
            var roles = resolvedData.Roles!;
            SelectedRoles = data.SelectedValues.Select(v => roles[v]).ToArray();
        }
        else
            SelectedRoles = Array.Empty<Role>();
    }
}

public class MentionableMenuInteractionContext : EntityMenuInteractionContext
{
    public override MentionableMenuInteraction Interaction { get; }

    public IReadOnlyList<Mentionable> SelectedMentionables { get; }

    public MentionableMenuInteractionContext(MentionableMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
        var data = Interaction.Data;
        if (data.ResolvedData != null)
        {
            var resolvedData = data.ResolvedData;
            var users = resolvedData.Users;
            var roles = resolvedData.Roles;
            if (users != null)
            {
                if (roles != null)
                    SelectedMentionables = data.SelectedValues.Select(v => users.TryGetValue(v, out var user) ? new Mentionable(user) : new Mentionable(roles[v])).ToArray();
                else
                    SelectedMentionables = data.SelectedValues.Select(v => new Mentionable(users[v])).ToArray();
            }
            else
            {
                if (roles != null)
                    SelectedMentionables = data.SelectedValues.Select(v => new Mentionable(roles[v])).ToArray();
                else
                    SelectedMentionables = Array.Empty<Mentionable>();
            }
        }
        else
            SelectedMentionables = Array.Empty<Mentionable>();
    }
}

public class ChannelMenuInteractionContext : EntityMenuInteractionContext
{
    public override ChannelMenuInteraction Interaction { get; }

    public IReadOnlyList<Channel> SelectedChannels { get; }

    public ChannelMenuInteractionContext(ChannelMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
        var data = interaction.Data;
        var resolvedData = data.ResolvedData;
        if (resolvedData != null)
        {
            var channels = resolvedData.Channels!;
            SelectedChannels = data.SelectedValues.Select(v => channels[v]).ToArray();
        }
        else
            SelectedChannels = Array.Empty<Channel>();
    }
}

public class ModalSubmitInteractionContext : BaseModalSubmitInteractionContext, IUserContext, IGuildContext, IChannelContext
{
    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    public TextChannel? Channel => Interaction.Channel;

    public IReadOnlyList<TextInput> Components => Interaction.Data.Components;

    public ModalSubmitInteractionContext(ModalSubmitInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}
