using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace MyBot;

[GatewayEvent(nameof(GatewayClient.MessageReactionAdd))]
public class MessageReactionAddHandler : IGatewayEventHandler<MessageReactionAddEventArgs>
{
    private readonly RestClient _client;

    public MessageReactionAddHandler(RestClient client)
    {
        _client = client;
    }

    public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        await _client.SendMessageAsync(args.ChannelId, $"<@{args.UserId}> reacted with {args.Emoji.Name}!");
    }
}
