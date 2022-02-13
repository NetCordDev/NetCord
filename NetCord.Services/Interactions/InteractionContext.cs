namespace NetCord.Services.Interactions;

public abstract class InteractionContext : IContext
{
    public abstract Interaction Interaction { get; }

    public GatewayClient Client { get; }

    public InteractionContext(GatewayClient client)
    {
        Client = client;
    }
}

public class BaseButtonInteractionContext : InteractionContext
{
    public override ButtonInteraction Interaction { get; }

    public BaseButtonInteractionContext(ButtonInteraction interaction, GatewayClient client) : base(client)
    {
        Interaction = interaction;
    }
}

public class BaseMenuInteractionContext : InteractionContext
{
    public override MenuInteraction Interaction { get; }

    public BaseMenuInteractionContext(MenuInteraction interaction, GatewayClient client) : base(client)
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

public class MenuInteractionContext : BaseMenuInteractionContext, IUserContext, IGuildContext, IChannelContext, IRestMessageContext
{
    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    public TextChannel Channel => Interaction.Channel!;

    public MenuInteractionContext(MenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}

public class ModalSubmitInteractionContext : BaseModalSubmitInteractionContext, IUserContext, IGuildContext, IChannelContext
{
    public User User => Interaction.User;

    public Guild? Guild => Interaction.Guild;

    public TextChannel Channel => Interaction.Channel!;

    public ModalSubmitInteractionContext(ModalSubmitInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}