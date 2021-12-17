namespace NetCord.Commands;

public interface ICommandContext
{
    public Message Message { get; }
    public Guild? Guild { get; }
    public BotClient Client { get; }
}