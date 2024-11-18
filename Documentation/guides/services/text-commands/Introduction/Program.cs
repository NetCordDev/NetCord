using NetCord;
using NetCord.Gateway;
using NetCord.Services;
using NetCord.Services.Commands;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent,
});

// Create the command service
CommandService<CommandContext> commandService = new();

// Add commands using minimal APIs
commandService.AddCommand(["ping"], () => "Pong!");

// Add commands from modules
commandService.AddModules(typeof(Program).Assembly);

// Add the handler to handle commands
client.MessageCreate += async message =>
{
    // Check if the message is a command (starts with '!' and is not from a bot)
    if (!message.Content.StartsWith('!') || message.Author.IsBot)
        return;

    // Execute the command
    var result = await commandService.ExecuteAsync(prefixLength: 1, new CommandContext(message, client));

    // Check if the execution failed
    if (result is not IFailResult failResult)
        return;

    // Return the error message to the user if the execution failed
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
