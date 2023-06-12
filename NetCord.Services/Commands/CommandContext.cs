using NetCord.Gateway;

namespace NetCord.Services.Commands;

public class BaseCommandContext : ICommandContext
{
    public BaseCommandContext(Message message, GatewayClient client)
    {
        Message = message;
        Client = client;
    }

    public Message Message { get; }
    public GatewayClient Client { get; }
}

public class CommandContext : BaseCommandContext, IGuildContext, IChannelContext, IUserContext
{
    public CommandContext(Message message, GatewayClient client) : base(message, client)
    {
    }

    public Guild? Guild => Message.Guild;
    public TextChannel? Channel => Message.Channel;
    public User User => Message.Author;
}
