using NetCord;
using NetCord.Gateway;

namespace MyBot;

internal class RegisteringHandlers
{
    public static async Task RegisterHandlersAsync()
    {
        ShardedGatewayClient client = new(new BotToken("Token from Discord Developer Portal"));

        client.Log += (client, message) =>
        {
            Console.WriteLine($"#{client.Shard.GetValueOrDefault().Id}\t{message}");
            return default;
        };

        client.MessageUpdate += async (client, message) =>
        {
            await message.ReplyAsync("Message updated!");
        };

        await client.StartAsync();
        await Task.Delay(-1);
    }
}
