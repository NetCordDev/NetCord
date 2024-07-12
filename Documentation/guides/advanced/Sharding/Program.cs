using NetCord;
using NetCord.Gateway;

ShardedGatewayClient shardedGatewayClient = new(new BotToken("Token from Discord Developer Portal"));

await shardedGatewayClient.StartAsync();
await Task.Delay(-1);
