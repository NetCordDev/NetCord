![NetCord](https://raw.githubusercontent.com/NetCordDev/NetCord/alpha/Resources/Logo/png/BigOutline.png)

## The modern and fully customizable C# Discord library

# Table of Contents

1. [📦 Installation](#installation)
2. [🚀 Showcase](#showcase)
3. [🎨 Features](#features)
4. [🥅 Goals](#goals)
5. [📚 Guides](#guides)
6. [📄 Documentation](#documentation)
7. [🩹 Support](#support)
6. [📜 License](#license)

## 1. 📦 Installation

You can install NetCord packages via NuGet package manager:

| Package                                                                                     | Description                                                                  |
|---------------------------------------------------------------------------------------------|------------------------------------------------------------------------------|
| **[NetCord](https://www.nuget.org/packages/NetCord)**                                       | Core package with fundamental functionality.                                 |
| **[NetCord.Services](https://www.nuget.org/packages/NetCord.Services)**                     | Facilitates seamless handling of commands and interactions.                  |
| **[NetCord.Hosting](https://www.nuget.org/packages/NetCord.Hosting)**                       | Provides .NET Generic Host extensions for the NetCord package.               |
| **[NetCord.Hosting.Services](https://www.nuget.org/packages/NetCord.Hosting.Services)**     | Provides .NET Generic Host extensions for the NetCord.Services package.      |
| **[NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore)** | Provides ASP.NET Core extensions for seamless handling of HTTP interactions. |

## 2. 🚀 Showcase

This snippet showcases a bot with a minimal API-style `/square` command and includes a module-based `/greet` command.

### Minimal API-style Bot Example

The following example sets up a bot with a minimal API-style approach for the `/square` command, which calculates the square of a number:

```cs
var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway()
    .UseApplicationCommands<SlashCommandInteraction, SlashCommandContext>();

var host = builder.Build()
    .AddSlashCommand<SlashCommandContext>("square", "Square!", (int a) => $"{a}² = {a * a}")
    .UseGatewayEventHandlers();

await host.RunAsync();
```

### Module-based Command Example

Moreover, you can use a module-based approach. Here's an example of a `/greet` command that greets a specified user:

```cs
public class GreetingModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("greet", "Greet someone!")]
    public string Greet(User user) => $"{Context.User} greets {user}!";
}
```

## 3. 🎨 Features

- **Fully Customizable** - Easily tailor your Discord bot with NetCord's flexible, extensible API.
- **Easy to Use** - Designed to be intuitive and easy to use for C# and .NET developers.
- **Lightweight & High Performance** - Optimized for efficient resource use, ideal for scalable bots.
- **Fully Asynchronous** - Built for responsive, concurrent operations in your bot.
- **Native AOT Support** - Enjoy faster startups and reduced memory usage with AOT compilation.
- **Immutable Caching** - Default immutable caching keeps data reliable and consistent.
- **Voice Support** - Includes high-quality capabilities for sending and receiving voice.
- **HTTP Interactions** - Easily handle interactions over HTTP without a persistent connection.
- **Dependency-Free** - Lightweight by design, with no external dependencies required.
- **Stateless REST** - Efficiently manage API requests with a stateless design.
- **Complete API Coverage** - Comprehensive support for the Discord API and advanced features.
- **Open Source** - Free to use and contribute to, licensed under MIT.

## 4. 🥅 Goals

NetCord's goal is to allow .NET developers to create fully customizable Discord bots without fighting the API wrapper itself. NetCord is designed to be easy to use and fully customizable, while still being lightweight and performant.

## 5. 📚 Guides

- **[Getting Started](https://netcord.dev/guides/getting-started/installation.html)**

## 6. 📄 Documentation

- **[Documentation](https://netcord.dev/docs/index.html)**

## 7. 🩹 Support

- **[Discord](https://discord.gg/meaSHTGyUH)**

## 8. 📜 License

This repository is released under the [MIT License](https://github.com/NetCordDev/NetCord/blob/alpha/LICENSE.md).
