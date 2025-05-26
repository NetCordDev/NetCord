using NetCord;
using NetCord.Gateway;
using NetCord.Logging;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new GatewayClientConfiguration
{
    Logger = new ConsoleLogger(),
});

await client.StartAsync();
await Task.Delay(-1);
