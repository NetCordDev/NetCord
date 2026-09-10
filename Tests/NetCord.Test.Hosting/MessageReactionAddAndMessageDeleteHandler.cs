using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace NetCord.Test.Hosting;

internal class MessageReactionAddAndMessageDeleteHandler : IMessageReactionAddGatewayHandler, IMessageDeleteGatewayHandler
{
    private readonly ILogger<MessageReactionAddAndMessageDeleteHandler> _logger;

    public MessageReactionAddAndMessageDeleteHandler(ILogger<MessageReactionAddAndMessageDeleteHandler> logger)
    {
        logger.LogInformation($"{nameof(MessageReactionAddAndMessageDeleteHandler)} created");

        _logger = logger;
    }

    public ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        _logger.LogInformation("Reaction {Name} added for message {MessageId} in channel {ChannelId} by user {UserId}", args.Emoji.Name, args.MessageId, args.ChannelId, args.UserId);

        return default;
    }

    public ValueTask HandleAsync(MessageDeleteEventArgs args)
    {
        _logger.LogInformation("Message {MessageId} deleted in channel {ChannelId}", args.MessageId, args.ChannelId);

        return default;
    }
}
