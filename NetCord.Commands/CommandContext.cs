namespace NetCord.Commands;

public class CommandContext : ICommandContext
{
    public UserMessage Message { get; }
    public TextChannel Channel => Message.Channel;
    public User User => Message.Author;
    public Guild? Guild => Message.Guild;
    public SocketClient Client { get; }

    public CommandContext(UserMessage message, SocketClient client)
    {
        Message = message;
        Client = client;
    }
}