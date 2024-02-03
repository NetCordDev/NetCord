using NetCord;
using NetCord.Gateway;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"));

client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};

await client.StartAsync();
await Task.Delay(-1);
