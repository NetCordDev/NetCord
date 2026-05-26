<p align="center">
    <img src="Resources/Logo/svg/BigOutline.svg" alt="Logo" width="650px">
</p>
<p align="center">
    <b>The modern and fully customizable C# Discord library</b>
</p>

<p align="center">
    ⭐ Found NetCord helpful or inspiring? Show your support by giving it a star! ⭐
</p>

# Table of Contents

1. [📦 Installation](#1--installation)
2. [🚀 Showcase](#2--showcase)
3. [🎨 Features](#3--features)
4. [🥅 Goals](#4--goals)
5. [📚 Guides](#5--guides)
6. [📄 Documentation](#6--documentation)
7. [🩹 Support](#7--support)
8. [📜 License](#8--license)
9. [🛠️ Development](#9-%EF%B8%8F-development)

## 1. 📦 Installation

You can install NetCord packages via NuGet package manager:

| Package                                                                                     | Description                                                             |
|---------------------------------------------------------------------------------------------|-------------------------------------------------------------------------|
| **[NetCord](https://www.nuget.org/packages/NetCord)**                                       | Core package with fundamental functionality.                            |
| **[NetCord.Services](https://www.nuget.org/packages/NetCord.Services)**                     | Facilitates seamless handling of commands and interactions.             |
| **[NetCord.Hosting](https://www.nuget.org/packages/NetCord.Hosting)**                       | Provides .NET Generic Host extensions for the NetCord package.          |
| **[NetCord.Hosting.Services](https://www.nuget.org/packages/NetCord.Hosting.Services)**     | Provides .NET Generic Host extensions for the NetCord.Services package. |
| **[NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore)** | Provides ASP.NET Core extensions for seamless handling of HTTP events.  |
| NetCord.Natives.&lt;RuntimeId&gt; | Provides pre-built native runtime dependencies binary.<br/>[See Native Dependencies Installation Guide.](https://netcord.dev/guides/basic-concepts/installing-native-dependencies.html) |

## 2. 🚀 Showcase

This snippet showcases a bot with a minimal API-style `/square` command and includes a module-based `/greet` command.

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

The following example sets up a bot with a minimal API-style approach for the `/square` command, which calculates the square of a number:

```cs
var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway()
    .UseApplicationCommands();

var host = builder.Build();

host.AddSlashCommand("square", "Square!", (int a) => $"{a}² = {a * a}");

await host.RunAsync();
```

Of course, you can also use the bare-bones approach.

### Module-based Command Example

Moreover, you can use a module-based approach. Here's an example of a `/greet` command that greets a specified user:

```cs
public class GreetingModule : ApplicationCommandModule<ApplicationCommandContext>
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

## 6. 📄 API Documentation

- **[API Documentation](https://netcord.dev/docs/)**

## 7. 🩹 Support

<a href="https://discord.gg/meaSHTGyUH"><img src="https://discord.com/api/guilds/988888771187581010/widget.png?style=banner2" alt="Discord"></a>

## 8. 📜 License

This repository is released under the [MIT License](LICENSE.md).

The use of NetCord.Natives may subject to each libraries licenses.

## 9. 🛠️ Development

### Versioning

NetCord follows a `MAJOR.MINOR.PATCH` versioning scheme:

- MAJOR versions can introduce significant, breaking API changes.
- MINOR versions can add new features and may include limited breaking changes that are unlikely to affect most users.
- PATCH versions contain backwards-compatible bug fixes and improvements.

Our goal is to maintain stability while allowing for necessary evolution of the API.
