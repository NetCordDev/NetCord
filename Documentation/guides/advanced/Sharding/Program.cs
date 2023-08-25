using NetCord;
using NetCord.Gateway;

ShardedGatewayClient shardedGatewayClient = new(new Token(TokenType.Bot, "Token"));

shardedGatewayClient.Log += (client, message) =>
{
    Console.WriteLine($"#{client.Shard.GetValueOrDefault().Id}\t{message}");
    return default;
};

await shardedGatewayClient.StartAsync();
await Task.Delay(-1);
