---
title: Introduction to NetCord - Modern C# Discord Bot Library for .NET
description: Learn about NetCord, a modern, fully customizable C# library for building Discord bots with .NET. Discover features and architecture.
omitAppTitle: true
keywords: Discord, NetCord, C#, .NET, bot library, introduction, features, architecture
section: Introduction
published_time: '2025-12-15T00:00:00Z'
modified_time: '2025-12-15T00:00:00Z'
---

# Introduction to NetCord

NetCord is a modern, fully customizable C# library for building Discord bots on .NET. It combines type safety, async/await throughout, dependency injection support, and a clean API—all without external dependencies beyond what .NET provides.

Whether you're building a simple utility bot, a complex multi-server bot with voice support, or a serverless bot using HTTP interactions, NetCord scales with your needs and stays out of your way.

## What is NetCord? {#what-is-netcord}

NetCord is a low-level Discord API wrapper for C# that provides direct access to Discord's REST and Gateway APIs. Unlike higher-level frameworks that make design decisions for you, NetCord gives you the tools to build exactly what you need with minimal restrictions.

### Key Design Principles

**Type-Safe by Default**
- Strongly-typed entities for every Discord concept (users, guilds, roles, channels, etc.)
- Compile-time checking prevents entire categories of runtime bugs
- Discoverability through IDE autocomplete

**Fully Asynchronous**
- Every I/O operation uses async/await patterns
- No thread-blocking operations
- Scales efficiently to thousands of concurrent operations

**No External Dependencies**
- Built on only the Microsoft.Extensions libraries (DI, Hosting, Configuration)
- Minimal surface area means fewer vulnerabilities and easier auditing
- Self-contained deployment without dependency conflicts

**Immutable by Default**
- Cache data is immutable, preventing accidental shared state bugs
- Safe for multi-threaded access without locks
- Clear ownership and lifetime semantics

**Highly Customizable**
- Extend every layer: REST requests, event handlers, command routing
- Bring your own database, caching, or logging
- Compose complex behaviors from simple building blocks

## Why Choose NetCord? {#why-choose-netcord}

### For .NET Developers

If you already work with .NET, NetCord lets you leverage your existing ecosystem:

- Use your familiar dependency injection container and configuration system
- Integrate with Entity Framework Core for databases
- Deploy to Azure, AWS, or any .NET-supporting platform
- Use LINQ, async streams, and modern C# features (records, nullable reference types, source generators)

### For Discord Bot Development

NetCord provides everything you need for Discord:

- **Complete API Coverage** - Every Discord API endpoint and event is accessible
- **Gateway & REST** - Connect via WebSocket for real-time events or use REST for HTTP interactions
- **Commands & Interactions** - Slash commands, message commands, user commands, and text commands with automatic validation
- **Components** - Buttons, select menus, modals, and the new Components v2 system
- **Voice** - Full voice channel support including audio encoding/decoding
- **Webhooks** - Webhook execution and event handling
- **Sharding** - Automatic shard management for large bots

### Not Your First Discord Bot?

Coming from another library? NetCord maintains philosophical compatibility:

- Similar concepts to Discord.NET but with cleaner APIs
- Similar to Disqord but more integrated with .NET hosting
- Inspired by JDA's architecture but with C# idioms
- Better performance than competitors through careful API design

## How NetCord Works {#how-netcord-works}

### The Two APIs: REST and Gateway

Discord is accessible through two independent APIs:

**REST API** - Your bot sends HTTP requests to Discord's servers
- Perfect for one-off operations (create a role, send a message)
- No persistent connection needed
- Handles HTTP interactions for serverless environments
- Subject to rate limiting (recoverable, automatic)

**Gateway API** - Your bot opens a WebSocket connection to Discord
- Receives real-time events (message created, user joined, etc.)
- Maintains connection state (cached guild data, voice state)
- Required for responding to user interactions (slash command clicks)
- Optional for bot-only operations

NetCord lets you use either or both, depending on your needs.

### Commands and Interactions

Discord's modern command system uses **Application Commands** and **Interactions**:

**Application Commands** - User-facing commands in Discord's UI:
- *Slash commands* (`/ping`) - typed parameters, autocomplete
- *User commands* - right-click a user to invoke
- *Message commands* - right-click a message to invoke

**Interactions** - Responses to user actions:
- Command invocations
- Button clicks
- Select menu choices
- Modal submissions

All are handled through NetCord's unified interaction system with the same context object and dependency injection.

### Text Commands (Legacy)

NetCord also supports text commands (`!ping`) for bots that need to maintain existing command syntax:

- Simple prefix-based routing
- Type readers for automatic parameter conversion
- Preconditions for permissions and restrictions

Text commands are optional—modern bots typically use slash commands and buttons for better UX.

## Core Packages {#core-packages}

NetCord is distributed as focused NuGet packages. Choose based on your bot architecture:

### Essential Package

| Package | Use When |
|---------|----------|
| **[NetCord](https://www.nuget.org/packages/NetCord)** | You need REST and/or Gateway access. This is the foundation for all Discord interaction. |

### When Adding Commands & Interactions

| Package | Use When |
|---------|----------|
| **[NetCord.Services](https://www.nuget.org/packages/NetCord.Services)** | You want automatic command registration, parameter validation, and interaction routing using attributes and modules. |

### When Using .NET Generic Host

These packages add convenient integration with .NET's hosting model (dependency injection, configuration, logging):

| Package | Use When |
|---------|----------|
| **[NetCord.Hosting](https://www.nuget.org/packages/NetCord.Hosting)** | You want Gateway integration with `IHostBuilder` / `HostApplicationBuilder` for dependency injection. |
| **[NetCord.Hosting.Services](https://www.nuget.org/packages/NetCord.Hosting.Services)** | You want both Gateway and command/interaction services integrated with hosting. |
| **[NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore)** | You're running a web server (ASP.NET Core) alongside your bot for HTTP interactions instead of Gateway. |

### What Should I Install?

**Just getting started?**
```
dotnet add package NetCord --prerelease
dotnet add package NetCord.Services --prerelease
dotnet add package NetCord.Hosting.Services --prerelease
```
This gives you everything: events, commands, interactions, and DI out of the box.

**Building a barebones bot?**
```
dotnet add package NetCord --prerelease
```
Manual Gateway connection, REST operations, and your own event handling. Minimal abstraction.

**Integrating with existing ASP.NET Core app?**
```
dotnet add package NetCord --prerelease
dotnet add package NetCord.Hosting.AspNetCore --prerelease
```
Receive Discord interactions as HTTP POST requests instead of maintaining a WebSocket.

## What Can You Build? {#what-can-you-build}

### Moderation Bot

Automatically enforce rules, log actions, and manage members:
- Respond to messages and filter content
- Assign/remove roles based on reactions
- Access audit logs and ban history
- Create threads for reports

### Utility Bot

Provide helpful tools your community needs:
- Fetch data (weather, crypto prices, quotes)
- Create dynamic components for user interaction
- Store preferences in a database
- Schedule recurring tasks

### Music/Voice Bot

Stream audio in voice channels:
- Process audio with Opus encoding
- Respond to button clicks in text chat
- Queue management and playback control
- Voice state tracking

### Moderation Dashboard

Web-based companion for admin tasks:
- View guild statistics
- Configure automated rules
- Review moderation logs
- Manage bot settings

All of these are possible with NetCord because of its completeness and flexibility.

## Key Features at a Glance {#features}

| Feature | Why It Matters |
|---------|---|
| **Strongly Typed** | Compile-time safety, IDE autocomplete, clearer code |
| **Fully Async** | Efficient resource use, scales to thousands of connections |
| **Dependency Injection** | Testability, clean architecture, reusable services |
| **Type-Safe Commands** | Parameters are validated at compile time and runtime |
| **Immutable Cache** | Safe multi-threaded access, predictable behavior |
| **Complete API** | Access any Discord API feature without workarounds |
| **Voice Support** | Full audio encoding/decoding with Opus |
| **Sharding** | Automatic shard management for large bots (2,500+ guilds) |
| **HTTP Interactions** | Run on serverless platforms (AWS Lambda, Azure Functions) |
| **No Dependencies** | Smaller attack surface, simpler deployments |
| **Highly Extensible** | Custom handlers, result processors, type readers, preconditions |
| **Modern C#** | Records, nullable references, source generators, async streams |

## The Two Paths to Learning {#choosing-your-path}

### Quick Start (5 Minutes)

You're comfortable with .NET and just want to see a working bot immediately.

- Create a new console project
- Install NetCord
- Copy a minimal example
- Run and test

**Best for:** Experienced .NET developers, quick prototyping, validating ideas

➜ Go to [Quick Start](../quick-start/index.md)

### Getting Started (30 Minutes)

You're new to NetCord or want to understand the fundamentals before diving in.

- Understand prerequisites (.NET 9+, bot application setup)
- Step-by-step installation walkthrough
- Detailed project structure explanation
- Your first working bot with explanations
- Next steps for learning commands, interactions, and more

**Best for:** New developers, learning the concepts, building a solid foundation

➜ Go to [Getting Started](../getting-started/installation.md)

## Next Steps {#next-steps}

1. **Follow one of the two paths above** to get your first bot running
2. **Explore [Commands](../commands/index.md)** to add slash commands and interactions
3. **Learn [Components](../component-interactions/message-components.md)** for buttons, menus, and modals
4. **Check [Gateway Events](../events/gateway-events.md)** to handle real-time Discord events
5. **Read [Advanced Topics](../advanced-topics/rate-limiting.md)** as you grow

## Questions or Stuck?

- Check the [Common Issues](../troubleshooting/common-issues.md) guide
- Visit the [NetCord Discord Community](https://discord.gg/meaSHTGyUH) for real-time help
- Report bugs on [GitHub](https://github.com/NetCord/NetCord/issues)
- Read the [API Documentation](https://netcord.dev/docs/) for detailed reference

---

## Navigation

← **Previous:** [FAQ](../troubleshooting/faq.md) | **Next:** [Quick Start](../quick-start/index.md) →

## See Also

- [Quick Start](../quick-start/index.md) - Get started in 5 minutes
- [Installation](../getting-started/installation.md) - Detailed setup guide
- [Migration Guide](../migration/index.md) - Coming from another library
- [GitHub Repository](https://github.com/NetCord/NetCord) - Source code and examples
- [Discord Server](https://discord.gg/netcord) - Community support

