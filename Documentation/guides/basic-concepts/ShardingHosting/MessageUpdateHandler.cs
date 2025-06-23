using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace MyBot;

public class MessageUpdateHandler : IMessageUpdateShardedGatewayEventHandler
{
    public async ValueTask HandleAsync(GatewayClient client, Message message)
    {
        await message.ReplyAsync("Message updated!");
    }
}
