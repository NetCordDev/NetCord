# Sharding

## When to shard?

Sharding is required when your bot is in more than 2500 guilds.

## What is sharding?

Sharding is splitting your bot into multiple @"NetCord.Gateway.GatewayClient"s. Each shard, represented as a @NetCord.Gateway.GatewayClient instance, will be responsible for a certain amount of guilds.

## How to shard?

To start sharding, you need to create an instance of @NetCord.Gateway.ShardedGatewayClient. Its usage is very similar to @NetCord.Gateway.GatewayClient. Example:
[!code-cs[Program.cs](Sharding/Program.cs)]

As you can see, the only difference in usage is that each event has an additional parameter, which is the @NetCord.Gateway.GatewayClient the event has been received from. Also note that events can be invoked concurrently from different shards.