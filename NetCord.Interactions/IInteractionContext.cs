namespace NetCord.Interactions;

public interface IButtonInteractionContext
{
    public Message Message { get; }
    public BotClient Client { get; }
    public Guild? Guild { get; }
    public ButtonInteraction Interaction { get; }
}

public interface IMenuInteractionContext
{
    public Message Message { get; }
    public BotClient Client { get; }
    public Guild? Guild { get; }
    public MenuInteraction Interaction { get; }
}