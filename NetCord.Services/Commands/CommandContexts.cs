using NetCord.Gateway;

namespace NetCord.Services.Commands;

public class BaseCommandContext : ICommandContext
{
    public BaseCommandContext(Message message)
    {
        Message = message;
    }

    public Message Message { get; }
}

public class CommandContext : BaseCommandContext, IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public CommandContext(Message message, GatewayClient client) : base(message)
    {
        Client = client;
    }

    public GatewayClient Client { get; }
    public Guild? Guild => Message.Guild;
    public TextChannel? Channel => Message.Channel;
    public User User => Message.Author;
}
