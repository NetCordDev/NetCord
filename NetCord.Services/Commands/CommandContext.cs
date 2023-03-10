using NetCord.Gateway;

namespace NetCord.Services.Commands;

public class CommandContext : ICommandContext, IUserContext, IGuildContext, IChannelContext, IMessageContext
{
    public Message Message { get; }
    public TextChannel? Channel => Message.Channel;
    public User User => Message.Author;
    public Guild? Guild => Message.Guild;
    public GatewayClient Client { get; }
    public IServiceProvider? Services { get; }

    public CommandContext(Message message, GatewayClient client, IServiceProvider? services)
    {
        Services = services;
        Message = message;
        Client = client;
    }
}
