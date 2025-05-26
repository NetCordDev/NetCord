using NetCord;
using NetCord.Gateway;
using NetCord.Logging;

namespace MyBot;

internal class RegisteringHandlers
{
    public static async Task RegisterHandlersAsync()
    {
        ShardedGatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new ShardedGatewayClientConfiguration
        {
            LoggerFactory = ShardedConsoleLogger.GetFactory(),
        });

        client.MessageUpdate += async (client, message) =>
        {
            await message.ReplyAsync("Message updated!");
        };

        await client.StartAsync();
        await Task.Delay(-1);
    }
}
