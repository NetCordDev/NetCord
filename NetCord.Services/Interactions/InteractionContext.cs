using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.Interactions;

public abstract class InteractionContext : IContext
{
    public InteractionContext(GatewayClient client)
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

    public TextChannel Channel => Interaction.Channel!;

    public ButtonInteractionContext(ButtonInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}

public class StringMenuInteractionContext : BaseStringMenuInteractionContext, IUserContext, IGuildContext, IChannelContext, IRestMessageContext
{
    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    public TextChannel Channel => Interaction.Channel!;

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

    public TextChannel Channel => Interaction.Channel!;

    public IReadOnlyList<Snowflake> SelectedValues => Interaction.Data.SelectedValues;

    public EntityMenuInteractionContext(EntityMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}

public class UserMenuInteractionContext : EntityMenuInteractionContext
{
    public override UserMenuInteraction Interaction { get; }

    public IReadOnlyDictionary<Snowflake, User> SelectedUsers { get; }

    public UserMenuInteractionContext(UserMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
        if (interaction.Data.ResolvedData != null)
            SelectedUsers = interaction.Data.SelectedValues.ToDictionary(v => v, v => interaction.Data.ResolvedData.Users![v]);
        else
            SelectedUsers = new Dictionary<Snowflake, User>(0);
    }
}

public class RoleMenuInteractionContext : EntityMenuInteractionContext
{
    public override RoleMenuInteraction Interaction { get; }

    public IReadOnlyDictionary<Snowflake, GuildRole> SelectedRoles { get; }

    public RoleMenuInteractionContext(RoleMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
        if (interaction.Data.ResolvedData != null)
            SelectedRoles = interaction.Data.SelectedValues.ToDictionary(v => v, v => interaction.Data.ResolvedData.Roles![v]);
        else
            SelectedRoles = new Dictionary<Snowflake, GuildRole>(0);
    }
}

public class MentionableMenuInteractionContext : EntityMenuInteractionContext
{
    public override MentionableMenuInteraction Interaction { get; }

    public IReadOnlyDictionary<Snowflake, Mentionable> SelectedMentionables { get; }

    public MentionableMenuInteractionContext(MentionableMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
        if (interaction.Data.ResolvedData != null)
        {
            Dictionary<Snowflake, Mentionable> selectedMentionables = new(interaction.Data.SelectedValues.Count);
            var users = interaction.Data.ResolvedData.Users;
            var roles = interaction.Data.ResolvedData.Roles;
            if (users != null)
            {
                if (roles != null)
                    SelectedMentionables = interaction.Data.SelectedValues.ToDictionary(v => v, v => users.TryGetValue(v, out var user) ? new Mentionable(user) : new Mentionable(roles[v]));
                else
                    SelectedMentionables = interaction.Data.SelectedValues.ToDictionary(v => v, v => new Mentionable(users[v]));
            }
            else
            {
                if (roles != null)
                    SelectedMentionables = interaction.Data.SelectedValues.ToDictionary(v => v, v => new Mentionable(roles[v]));
                else
                    SelectedMentionables = new Dictionary<Snowflake, Mentionable>(0);
            }
        }
        else
            SelectedMentionables = new Dictionary<Snowflake, Mentionable>(0);
    }
}

public class ChannelMenuInteractionContext : EntityMenuInteractionContext
{
    public override ChannelMenuInteraction Interaction { get; }

    public IReadOnlyDictionary<Snowflake, Channel> SelectedChannels { get; }

    public ChannelMenuInteractionContext(ChannelMenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
        Interaction = interaction;
        if (interaction.Data.ResolvedData != null)
            SelectedChannels = interaction.Data.SelectedValues.ToDictionary(v => v, v => interaction.Data.ResolvedData.Channels![v]);
        else
            SelectedChannels = new Dictionary<Snowflake, Channel>(0);
    }
}

public class ModalSubmitInteractionContext : BaseModalSubmitInteractionContext, IUserContext, IGuildContext, IChannelContext
{
    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    public TextChannel Channel => Interaction.Channel!;

    public IReadOnlyList<TextInput> Components => Interaction.Data.Components;

    public ModalSubmitInteractionContext(ModalSubmitInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}
