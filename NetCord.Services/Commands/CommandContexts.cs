using NetCord.Gateway;

namespace NetCord.Services.Commands;

/// <summary>
/// Base context for handling text-based commands.
/// </summary>
/// <param name="message"><inheritdoc cref="Message" path="/summary"/></param>
public class BaseCommandContext(Message message) : ICommandContext
{
    public Message Message => message;
}

/// <summary>
/// Context for handling text-based commands.
/// </summary>
/// <param name="message"><inheritdoc cref="BaseCommandContext.Message" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class CommandContext(Message message, GatewayClient client)
    : BaseCommandContext(message),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;

    public Guild? Guild => Message.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel? Channel => Message.Channel;

    public User User => Message.Author;

    ulong? IGuildContext.GuildId => Message.GuildId;
}
