using NetCord;
using NetCord.Gateway;
using NetCord.Logging;

ShardedGatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new ShardedGatewayClientConfiguration
{
    LoggerFactory = ShardedConsoleLogger.GetFactory(),
});

await client.StartAsync();
await Task.Delay(-1);
