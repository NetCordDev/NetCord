<p align="center">
    <img src="Resources/Logo/svg/BigOutline.svg" alt="Logo" width="650px">
</p>
<p align="center">
    <b>The modern and fully customizable C# Discord library</b>
</p>

# Table of Contents

1. [📦 Installation](#1--installation)
2. [🚀 Example](#2--example)
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

## 2. 🚀 Example

This snippet showcases a Discord bot with a minimal API-style `/ping` command responding with `Pong!`.

<details>
<summary>Usings omitted for readability, click here to show</summary>

```cs
using Microsoft.Extensions.Hosting;

using NetCord;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Services.ApplicationCommands;
```

</details>

```cs
var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway()
    .UseApplicationCommandService<SlashCommandInteraction, SlashCommandContext>();

var host = builder.Build()
    .AddSlashCommand<SlashCommandContext>("ping", "Ping!", () => "Pong!")
    .UseGatewayEventHandlers();

await host.RunAsync();
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
