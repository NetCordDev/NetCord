using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace NetCord.Test.Hosting;

[GatewayEvent(nameof(GatewayClient.MessageReactionAdd))]
internal class MessageReactionAddHandler : IGatewayEventHandler<MessageReactionAddEventArgs>
{
    private readonly ILogger<MessageReactionAddHandler> _logger;

    public MessageReactionAddHandler(ILogger<MessageReactionAddHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        _logger.LogInformation("Reaction {Name} added for message {MessageId} in channel {ChannelId} by user {UserId}", args.Emoji.Name, args.MessageId, args.ChannelId, args.UserId);

        return default;
    }
}
