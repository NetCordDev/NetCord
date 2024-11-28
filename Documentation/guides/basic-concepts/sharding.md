---
uid: sharding
omitAppTitle: true
title: Scale Your C# Discord Bot with Sharding in NetCord
description: Implement sharding for your C# Discord bot with NetCord to improve scalability and performance by distributing tasks across multiple gateway connections.
---

# Sharding

Sharding allows your bot to split its responsibilities across multiple gateway connections. In NetCord, this is managed by the @NetCord.Gateway.ShardedGatewayClient, which acts as a controller for multiple instances of @NetCord.Gateway.GatewayClient. Each shard, represented by a @NetCord.Gateway.GatewayClient, handles a specific subset of guilds.

## When to Shard

Sharding becomes necessary when your bot exceeds 2,500 guilds. However, it's recommended to implement sharding earlier, typically when targeting 1,000+ guilds, to ensure smoother scaling and performance.

## How to Shard

### [.NET Generic Host](#tab/generic-host)

When using the .NET Generic Host, you can add the @NetCord.Gateway.ShardedGatewayClient by calling @NetCord.Hosting.Gateway.ShardedGatewayClientServiceCollectionExtensions.AddDiscordShardedGateway*.
[!code-cs[Program.cs](ShardingHosting/Program.cs)]

### [Bare Bones](#tab/bare-bones)

For a bare-bones setup, you need to manually create an instance of @NetCord.Gateway.ShardedGatewayClient. Its API is very similar to @NetCord.Gateway.GatewayClient. Here's an example:
[!code-cs[Program.cs](Sharding/Program.cs)]

***

## How to Register Events

### [.NET Generic Host](#tab/generic-host)

To register event handlers with sharding in the .NET Generic Host, use @NetCord.Hosting.Gateway.GatewayEventHandlerServiceCollectionExtensions.AddShardedGatewayEventHandlers* to add all event handlers in an assembly and then call @NetCord.Hosting.Gateway.GatewayEventHandlerServiceCollectionExtensions.AddShardedGatewayEventHandlers* to bind these handlers to the sharded client.

[!code-cs[Program.cs](ShardingHosting/RegisteringHandlers.cs?highlight=10,13#L12-L26)]

When creating event handlers, implement @NetCord.Hosting.Gateway.IShardedGatewayEventHandler or @NetCord.Hosting.Gateway.IShardedGatewayEventHandler`1. Note the additional parameter representing the @NetCord.Gateway.GatewayClient that received the event.
[!code-cs[Program.cs](ShardingHosting/MessageUpdateHandler.cs)]

### [Bare Bones](#tab/bare-bones)

For bare-bones setups, adding event handlers is straightforward. Each handler has an additional parameter for the @NetCord.Gateway.GatewayClient that received the event.
[!code-cs[Program.cs](Sharding/RegisteringHandlers.cs#L18-L21)]
