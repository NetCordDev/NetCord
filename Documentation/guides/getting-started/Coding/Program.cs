using NetCord;
using NetCord.Gateway;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"));

await client.StartAsync();
await Task.Delay(-1);
