<p align="center">
    <img src="https://cdn.discordapp.com/attachments/800832361351872524/910967946933833738/BigOutline.png" alt="Logo">
</p>
<b>A lightweight and asynchronous Discord API wrapper.</b>
<h1>Nuget packages</h1>
<p align="center">
    <a href="https://www.nuget.org/packages/NetCord">
        <img src="https://img.shields.io/nuget/v/NetCord?color=5865F2&logo=nuget&label=NetCord&style=flat-square" alt="NuGet">
    </a>
    <a href="https://www.nuget.org/packages/NetCord.Services">
        <img src="https://img.shields.io/nuget/v/NetCord.Services?color=5865F2&logo=nuget&label=NetCord.Services&style=flat-square" alt="NuGet">
    </a>
</p>

<h1>Example bot</h1>

```c#
using NetCord;
using NetCord.Gateway;
using NetCord.Services.Commands;

// Creating a bot object
Token token = new(TokenType.Bot, Environment.GetEnvironmentVariable("token")!);
GatewayClientConfig config = new()
{
    Intents = GatewayIntent.GuildMessages | GatewayIntent.DirectMessages | GatewayIntent.MessageContent
};
GatewayClient client = new(token, config);

// Logging into the console
client.Log += message =>
{
    Console.WriteLine(message);
    return default;
};

// Creating a command service
CommandService<CommandContext> commandService = new();
// Adding modules to the command service
commandService.AddModules(System.Reflection.Assembly.GetEntryAssembly()!);

// Handling commands
client.MessageCreate += async message =>
{
    // We don't want bots to use our commands
    if (!message.Author.IsBot)
    {
        const string prefix = "!";

        // Checking if message starts with prefix
        if (message.Content.StartsWith(prefix))
        {
            try
            {
                // Executing a command
                CommandContext context = new(message, client);
                await commandService.ExecuteAsync(prefix.Length, context);
            }
            catch (Exception ex)
            {
                try
                {
                    // Showing error in case of error
                    await message.ReplyAsync(ex.Message, failIfNotExists: false);
                }
                catch
                {
                }
            }
        }
    }
};

// Connecting our bot
await client.StartAsync();
// We don't want our bot to close immediately
await Task.Delay(-1);

public class ExampleModule : CommandModule<CommandContext>
{
    [Command("ping")]
    public Task PingAsync()
    {
        return ReplyAsync("pong!");
    }
}
```
