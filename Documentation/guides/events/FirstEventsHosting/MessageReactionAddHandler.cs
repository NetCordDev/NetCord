using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace MyBot;

[GatewayEvent(nameof(GatewayClient.MessageReactionAdd))]
public class MessageReactionAddHandler(RestClient client) : IGatewayEventHandler<MessageReactionAddEventArgs>
{
    public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        await client.SendMessageAsync(args.ChannelId, $"<@{args.UserId}> reacted with {args.Emoji.Name}!");
    }
}
