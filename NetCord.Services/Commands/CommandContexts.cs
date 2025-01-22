using NetCord.Gateway;

namespace NetCord.Services.Commands;

public class BaseCommandContext(Message message) : ICommandContext
{
    public Message Message => message;
}

public class CommandContext(Message message, GatewayClient client)
    : BaseCommandContext(message),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;
    public Guild? Guild => Message.Guild;
    public TextChannel? Channel => Message.Channel;
    public User User => Message.Author;

    ulong? IGuildContext.GuildId => Message.GuildId;
}
