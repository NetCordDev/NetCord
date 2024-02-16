using Microsoft.Extensions.Logging;

using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace NetCord.Test.Hosting;

[GatewayEvent(nameof(GatewayClient.MessageReactionAdd))]
[GatewayEvent(nameof(GatewayClient.MessageDelete))]
internal class MessageReactionAddAndMessageDeleteHandler(ILogger<MessageReactionAddAndMessageDeleteHandler> logger) : IGatewayEventHandler<MessageReactionAddEventArgs>, IGatewayEventHandler<MessageDeleteEventArgs>
{
    public ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        logger.LogInformation("Reaction {Name} added for message {MessageId} in channel {ChannelId} by user {UserId}", args.Emoji.Name, args.MessageId, args.ChannelId, args.UserId);

        return default;
    }

    public ValueTask HandleAsync(MessageDeleteEventArgs args)
    {
        logger.LogInformation("Message {MessageId} deleted in channel {ChannelId}", args.MessageId, args.ChannelId);

        return default;
    }
}
