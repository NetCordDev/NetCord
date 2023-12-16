using NetCord;
using NetCord.Gateway;
using NetCord.Services.Commands;

GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent,
});

CommandService<CommandContext> commandService = new();
commandService.AddModules(typeof(Program).Assembly);

client.MessageCreate += async message =>
{
    if (message.Content.StartsWith("!"))
    {
        try
        {
            await commandService.ExecuteAsync(prefixLength: 1, new CommandContext(message, client));
        }
        catch (Exception ex)
        {
            try
            {
                await message.ReplyAsync($"Error: {ex.Message}");
            }
            catch
            {
            }
        }
    }
};
client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};
await client.StartAsync();
await Task.Delay(-1);
