# Introduction

Firstly, add the following line to using section.
```cs
using NetCord.Services.Commands;
```

Now, it's time to create @NetCord.Services.Commands.CommandService`1 instance and add modules to it.
```cs
CommandService<CommandContext> commandService = new();
commandService.AddModules(System.Reflection.Assembly.GetEntryAssembly()!);
```

We can add command handler now.
```cs
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
                await message.ReplyAsync($"Error: {ex.Message}", failIfNotExists: false);
            }
            catch
            {
            }
        }
    }
};
```

Now, if you write a message starting with `!`, a bot will reply with `Error: Command not found.`. It's time to create first commands!
Create a new file `FirstModule.cs`. Make sure the class is public and add `using NetCord.Services.Commands;`. Make the class inheriting from `CommandModule<CommandContext>`. The file should look like this:
```cs
using NetCord.Services.Commands;

namespace MyBot;

public class FirstModule : CommandModule<CommandContext>
{
}
```

Now, we will create a `ping` command! Add the following lines to the class.
```cs
[Command("ping")]
public Task PingAsync()
{
    return ReplyAsync("pong!");
}
```
Now, you have your first command working!

## The Final Product

# [Program.cs](#tab/program)
```cs
using NetCord;
using NetCord.Gateway;
using NetCord.Services.Commands;

GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"), new GatewayClientConfig()
{
	Intents = GatewayIntent.GuildMessages | GatewayIntent.DirectMessages | GatewayIntent.MessageContent
});

CommandService<CommandContext> commandService = new();
commandService.AddModules(System.Reflection.Assembly.GetEntryAssembly()!);

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
                await message.ReplyAsync($"Error: {ex.Message}", failIfNotExists: false);
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
```

# [FirstModule.cs](#tab/first-module)
```cs
using NetCord.Services.Commands;

namespace MyBot;

public class FirstModule : CommandModule<CommandContext>
{
    [Command("ping")]
    public Task PingAsync()
    {
        return ReplyAsync("pong!");
    }
}
```