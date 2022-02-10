namespace NetCord.Services.Commands;

public class CommandContext : ICommandContext, IUserContext, IGuildContext, IChannelContext, IMessageContext
{
    public Message Message { get; }
    public TextChannel Channel => Message.Channel;
    public User User => Message.Author;
    public Guild? Guild => Message.Guild;
    public GatewayClient Client { get; }

    public CommandContext(Message message, GatewayClient client)
    {
        Message = message;
        Client = client;
    }
}