namespace NetCord.Commands;

public class CommandContext : ICommandContext
{
    public UserMessage Message { get; }
    public TextChannel Channel => Message.Channel;
    public User User => Message.Author;
    public Guild? Guild => Message.Guild;
    public BotClient Client { get; }

    public CommandContext(UserMessage message, BotClient client)
    {
        Message = message;
        Client = client;
    }
}