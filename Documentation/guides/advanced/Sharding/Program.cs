using NetCord;
using NetCord.Gateway;

ShardedGatewayClient shardedGatewayClient = new(new BotToken("Token from Discord Developer Portal"));

shardedGatewayClient.Log += (client, message) =>
{
    Console.WriteLine($"#{client.Shard.GetValueOrDefault().Id}\t{message}");
    return default;
};

await shardedGatewayClient.StartAsync();
await Task.Delay(-1);
