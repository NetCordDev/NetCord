namespace NetCord.Interactions;

public class ButtonInteractionContext : IButtonInteractionContext
{
    public RestMessage Message => Interaction.Message;

    public Guild? Guild => Interaction.Guild;

    public BotClient Client { get; }

    public User User => Interaction.User;

    public ButtonInteraction Interaction { get; }

    public ButtonInteractionContext(ButtonInteraction interaction, BotClient client)
    {
        Interaction = interaction;
        Client = client;
    }
}

public class MenuInteractionContext : IMenuInteractionContext
{
    public RestMessage Message => Interaction.Message;

    public Guild? Guild => Interaction.Guild;

    public BotClient Client { get; }

    public User User => Interaction.User;

    public MenuInteraction Interaction { get; }

    public MenuInteractionContext(MenuInteraction interaction, BotClient client)
    {
        Interaction = interaction;
        Client = client;
    }
}