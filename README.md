<p align="center">
    <img src="Resources/Logo/svg/BigOutline.svg" alt="Logo" width="650px">
</p>
<p align="center">
    <b>The modern and fully customizable C# Discord library</b>
</p>

<p align="center">
    ⭐ If you like this project, please consider giving it a star! ⭐
</p>

# Table of Contents

1. [📦 Installation](#1--installation)
2. [🚀 Showcase](#2--showcase)
3. [🎨 Features](#3--features)
4. [🥅 Goals](#4--goals)
5. [📚 Guides](#5--guides)
6. [📄 Documentation](#6--documentation)
7. [🩹 Support](#7--support)
6. [📜 License](#8--license)

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

This snippet showcases a bot with a minimal API-style `/sum` command and includes a module-based `/greet` command.

<details>
<summary>Required usings omitted for readability, click here to show</summary>

```cs
using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Services.ApplicationCommands;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
```

</details>

### Minimal API-style Bot Example

The following example sets up a bot with a minimal API-style approach for the `/sum` command, which calculates the sum of two numbers:

```cs
var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway()
    .UseApplicationCommands<SlashCommandInteraction, SlashCommandContext>();

var host = builder.Build()
    .AddSlashCommand<SlashCommandContext>("sum", "Sum numbers!", (decimal a, decimal b) => $"{a} + {b} = {a + b}")
    .UseGatewayEventHandlers();

await host.RunAsync();
```

### Module-based Command Example

Moreover, you can enhance your bot with module-based commands. Here's an example of a `/greet` command that greets a specified user:

```cs
public class GreetingModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("greet", "Greet someone!")]
    public string Greet(User user) => $"{Context.User} greets {user}!";
}
```

## 3. 🎨 Features

- **Fully customizable** - NetCord is fully customizable and extensible
- **Easy to use** - NetCord is easy to use and understand
- **Lightweight** - NetCord is lightweight and performant
- **Asynchronous** - NetCord is fully asynchronous and concurrent
- **AOT-ready** - NetCord supports Native AOT compilation
- **Immutable** - NetCord's caching is immutable by default
- **Voice-ready** - NetCord supports both sending and receiving voice
- **Comprehensive** - NetCord covers the entire Discord API and more

## 4. 🥅 Goals

NetCord's goal is to allow .NET developers to create fully customizable Discord bots without fighting the API wrapper itself. NetCord is designed to be easy to use and fully customizable, while still being lightweight and performant.

## 5. 📚 Guides

- **[Getting Started](https://netcord.dev/guides/getting-started/installation.html)**

## 6. 📄 Documentation

- **[Documentation](https://netcord.dev/docs/index.html)**

## 7. 🩹 Support

<a href="https://discord.gg/meaSHTGyUH"><img src="https://discord.com/api/guilds/988888771187581010/widget.png?style=banner2" alt="Discord"></a>

## 8. 📜 License

This repository is released under the [MIT License](LICENSE.md).
