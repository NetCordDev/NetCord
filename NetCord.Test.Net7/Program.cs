using NetCord;
using NetCord.Gateway;

GatewayClient client = new(new(TokenType.Bot, Environment.GetEnvironmentVariable("token")!), new()
{
    Intents = GatewayIntent.GuildMessages | GatewayIntent.DirectMessages | GatewayIntent.MessageContent,
});
client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};
client.MessageCreate += async message =>
{
    if (!message.Author.IsBot && message.Content == "!hello")
        await message.ReplyAsync("Hello from Native AOT!");
};
await client.StartAsync();
await Task.Delay(-1);
