namespace NetCord.Services.Interactions;

public abstract class InteractionContext : IContext
{
    public abstract Interaction Interaction { get; }

    public Guild? Guild => Interaction.Guild;

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

public class ButtonInteractionContext : BaseButtonInteractionContext
{
    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    public ButtonInteractionContext(ButtonInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}

public class MenuInteractionContext : BaseMenuInteractionContext
{
    public RestMessage Message => Interaction.Message;

    public User User => Interaction.User;

    public MenuInteractionContext(MenuInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}