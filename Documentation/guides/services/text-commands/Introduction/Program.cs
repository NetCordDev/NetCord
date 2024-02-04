using NetCord;
using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent,
});

CommandService<CommandContext> commandService = new();
commandService.AddModules(typeof(Program).Assembly);

client.MessageCreate += async message =>
{
    if (!message.Content.StartsWith('!') || message.Author.IsBot)
        return;

    var result = await commandService.ExecuteAsync(prefixLength: 1, new CommandContext(message, client));

    if (result is not IFailResult failResult)
        return;

    try
    {
        await message.ReplyAsync(failResult.Message);
    }
    catch
    {
    }
};

client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};

await client.StartAsync();
await Task.Delay(-1);
