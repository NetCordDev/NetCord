using NetCord;
using NetCord.Gateway;

ShardedGatewayClient client = new(new BotToken("Token from Discord Developer Portal"));

client.Log += (client, message) =>
{
    Console.WriteLine($"#{client.Shard.GetValueOrDefault().Id}\t{message}");
    return default;
};

await client.StartAsync();
await Task.Delay(-1);
