---
uid: Intents
---

# Intents

## What are intents?
Intents allow you to subscribe Discord events, such as @NetCord.Gateway.GatewayClient.MessageCreate and @NetCord.Gateway.GatewayClient.GuildUserAdd. If you don't specify certain intent, you will not subscribe certain events.

## Privileged intents
Privileged intents are intents that you need to enable in [Discord Developer Portal](https://discord.com/developers/applications).
![](../../images/intents_Privileged.png)

## How to specify intents in NetCord?
Intents in NetCord are handled by @NetCord.Gateway.GatewayIntents.
You specify them like this:
```cs
GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent,
});
```

> [!WARNING]
> `MessageContent` intent is privileged.

If you made this, you receive guild and direct messages.

> [!NOTE]
> `MessageContent` is a special intent that allows you to receive @NetCord.Rest.RestMessage.Content, @NetCord.Rest.RestMessage.Embeds, @NetCord.Rest.RestMessage.Attachments and @NetCord.Rest.RestMessage.Components of messages on events. Otherwise they are empty.