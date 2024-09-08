# Sharding

## When to shard?

Sharding is required when your bot is in more than 2500 guilds.

## What is sharding?

Sharding is splitting your bot into multiple @"NetCord.Gateway.GatewayClient"s. Each shard, represented as a @NetCord.Gateway.GatewayClient instance, will be responsible for a certain number of guilds.

## How to shard?

## [Generic Host](#tab/generic-host)

To start sharding with the generic host, instead of calling @NetCord.Hosting.Gateway.GatewayClientHostBuilderExtensions.UseDiscordGateway(Microsoft.Extensions.Hosting.IHostBuilder), you need to call @NetCord.Hosting.Gateway.ShardedGatewayClientHostBuilderExtensions.UseDiscordShardedGateway(Microsoft.Extensions.Hosting.IHostBuilder). Example:
[!code-cs[Program.cs](ShardingHosting/Program.cs)]

Also note that you need to use @NetCord.Hosting.Gateway.IShardedGatewayEventHandler or @NetCord.Hosting.Gateway.IShardedGatewayEventHandler`1 instead of @NetCord.Hosting.Gateway.IGatewayEventHandler or @NetCord.Hosting.Gateway.IGatewayEventHandler`1 for event handlers. You also need to use @NetCord.Hosting.Gateway.GatewayEventHandlerServiceCollectionExtensions.AddShardedGatewayEventHandlers(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Reflection.Assembly) to add event handlers.

## [Bare Bones](#tab/bare-bones)

To start sharding, you need to create an instance of @NetCord.Gateway.ShardedGatewayClient. Its usage is very similar to @NetCord.Gateway.GatewayClient. Example:
[!code-cs[Program.cs](Sharding/Program.cs)]

As you can see, the only difference in usage is that each event has an additional parameter, which is the @NetCord.Gateway.GatewayClient the event has been received from. Also note that events can be invoked concurrently from different shards.