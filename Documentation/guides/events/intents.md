---
uid: events
---

# Intents

## What are intents?
Intents allow you to subscribe Discord events, such as @NetCord.Gateway.GatewayClient.MessageCreate and @NetCord.Gateway.GatewayClient.GuildUserAdd. If you don't specify certain intent, you will not subscribe certain events.

## Privileged intents
Privileged intents are intents that you need to enable in [Discord Developer Portal](https://discord.com/developers/applications).
![Shows 'Privileged Gateway Intents' section in 'Bot' section](../../images/intents_Privileged.webp){width=850px}

## How to specify intents in NetCord?

Intents in NetCord are handled by @NetCord.Gateway.GatewayIntents.
You specify them like this:

## [.NET Generic Host](#tab/generic-host)
[!code-cs[Program.cs](IntentsHosting/Program.cs?highlight=4#L8-L12)]

## [Bare Bones](#tab/bare-bones)
[!code-cs[Program.cs](Intents/Program.cs?highlight=3#L4-L7)]

***

If you have done this, you will receive guild and direct messages.

> [!NOTE]
> `MessageContent` is a special, privileged intent that allows you to receive @NetCord.Rest.RestMessage.Content, @NetCord.Rest.RestMessage.Embeds, @NetCord.Rest.RestMessage.Attachments, @NetCord.Rest.RestMessage.Components and @NetCord.Rest.RestMessage.Poll of messages in events. Otherwise they are empty.
