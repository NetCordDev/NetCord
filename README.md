<p align="center">
    <img src="Resources/Logo/png/BigOutline.png" alt="Logo">
</p>
<p align="center">
    <a href="https://www.nuget.org/packages/NetCord"><img src="https://img.shields.io/nuget/v/NetCord?color=5865F2&logo=nuget&label=NetCord" alt="NuGet"></a>
    <a href="https://www.nuget.org/packages/NetCord.Services"><img src="https://img.shields.io/nuget/v/NetCord.Services?color=5865F2&logo=nuget&label=NetCord.Services" alt="NuGet">
    </a>
</p>

**A lightweight and asynchronous Discord API wrapper.**

## 📄 Documentation

- **[netcord.dev](https://netcord.dev)**

## 📥 installation

- [NetCord](https://www.nuget.org/packages/NetCord/1.0.0-alpha.163)
```
Install-Package NetCord -Version 1.0.0-alpha.163
```

- [NetCord.Services](https://www.nuget.org/packages/NetCord.Services/1.0.0-alpha.85)
```
Install-Package NetCord.Services -Version 1.0.0-alpha.85
```

## 📝 Example usage

```cs
// Import necessary libraries from the NetCord namespace
using NetCord;
using NetCord.Gateway;
using NetCord.Services.Commands;

// Create a new GatewayClient, passing in the bot token and a configuration object
GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    // Specify the desired GatewayIntents; in this case, the bot will listen for messages from group and private chats, as well as message content
    Intents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent,
});

// Subscribe to the MessageCreate event of the GatewayClient
client.MessageCreate += async message =>
{
    if (message.Content.Contains("!hello"))
    {
        try
        {
            // Send a response message with a greeting and a mention of the user
            await message.ReplyAsync($"Hello, {message.Author.Mention}!");
        }
        catch (Exception ex)
        {
            try
            {
                // If there is an error sending the message, send an error message
                await message.ReplyAsync($"Error: {ex.Message}", failIfNotExists: false);
            }
            catch
            {
                // If it's not possible to send a message, do nothing
            }
        }
    }
};

// Set the logging function for the GatewayClient
client.Log += message =>
{
    // Output client log messages to the console
    Console.WriteLine(message);
    return default;
};

// Start the client and wait for new messages
await client.StartAsync();
await Task.Delay(-1);
```

## 🩹 Support

<a href="https://discord.gg/meaSHTGyUH"><img src="https://discord.com/api/guilds/988888771187581010/widget.png?style=banner2" alt="Discord"></a>
