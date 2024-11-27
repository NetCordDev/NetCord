using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace MyBot;

[GatewayEvent(nameof(GatewayClient.MessageUpdate))]
public class MessageUpdateHandler : IShardedGatewayEventHandler<Message>
{
    public async ValueTask HandleAsync(GatewayClient client, Message message)
    {
        await message.ReplyAsync("Message updated!");
    }
}
