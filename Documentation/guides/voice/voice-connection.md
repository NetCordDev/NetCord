---
title: Connecting Your Discord Bot to Voice Channels with NetCord
description: Join and manage Discord voice connections in C#. Learn voice state management, channel switching, and voice connection lifecycle.
omitAppTitle: true
keywords: Discord, voice channels, voice connections, VoiceClient, voice state, connection management, C#, .NET
section: Voice
published_time: '2025-12-15T00:00:00Z'
modified_time: '2026-02-12T00:00:00Z'
---

# Voice Connection Overview

In this section, you'll learn how to connect your Discord bot to voice channels using NetCord. We'll cover the basics of voice connections, managing voice state, and best practices for maintaining stable connections.

## Connecting to a Voice Channel {#voice-connection}

NetCord provides a high-level and a low-level API for connecting to voice channels. The high-level API is simpler to use and is suitable for most use cases, while the low-level API gives you more control over the connection process.

You should use the high-level API unless you have specific needs that require the low-level API.

### High-Level API

To connect to a voice channel using the high-level API, you can simply use the @NetCord.Gateway.Voice.GatewayClientExtensions.JoinVoiceChannelAsync* method. This method requires the guild ID and channel ID to be specified and updates the bot's voice state to join the channel. It returns a @NetCord.Gateway.Voice.VoiceClient instance, you can then call @NetCord.Gateway.Voice.VoiceClient.StartAsync* on it to establish the connection.

[!code-cs[Voice Connection High Level](VoiceConnection/Examples.cs#L11-L16)]

### Low-Level API

The low-level API requires you to write the connection logic yourself, instead of relying on @NetCord.Gateway.Voice.GatewayClientExtensions.JoinVoiceChannelAsync* which does it for you. You need to:
- Update the bot's voice state to join the channel using @NetCord.Gateway.GatewayClient.UpdateVoiceStateAsync*.
- Listen for the @NetCord.Gateway.GatewayClient.VoiceStateUpdate event to get the voice state of the bot, which contains the session ID and the endpoint and simultaneously listen for the @NetCord.Gateway.GatewayClient.VoiceServerUpdate event to get the token needed to connect to the voice server.
- Create a @NetCord.Gateway.Voice.VoiceClient instance with that data.
- Call @NetCord.Gateway.Voice.VoiceClient.StartAsync* to establish the connection.

<details>

<summary>Here you can see how NetCord handles that in high-level API</summary>

[!code-cs[Voice Connection Low Level](../../../NetCord/Gateway/Voice/GatewayClientExtensions.cs)]

</details>

<br />

## Voice Connection vs Voice State

It is important to understand the difference between a voice connection and a voice state. A voice connection represents an active connection to a voice channel, allowing you to send and receive audio. A voice state, on the other hand, represents the current state of a user in relation to voice channels. Interestingly enough, a bot can have a voice state in a channel without having an active voice connection.

You can update the bot's voice state using @NetCord.Gateway.GatewayClient.UpdateVoiceStateAsync*, you can specify the channel ID to join a channel, or set it to null to leave. The last one is useful for leaving a channel, since the @NetCord.Gateway.Voice.VoiceClient manages the voice connection, not the voice state. If you call @NetCord.Gateway.WebSocketClient.CloseAsync*, the voice connection will be closed, but the bot will still have a voice state in the channel for some time, unless you explicitly update the voice state to leave.

Below you can see an example of leaving a voice channel while updating the voice state so that the bot doesn't appear as still being in the channel.

[!code-cs[Leaving Voice Channel](VoiceConnection/Examples.cs#L21-L23)]

## Updating Our Bot

Let's use our knowledge of voice connections to update our bot so that it can join and leave voice channels. We will add two commands, `/join` and `/leave`, to handle these actions.

Below you can see the implementation of the `/join` command, which uses the high-level API to join a voice channel. It accepts an optional channel parameter - if provided, the bot joins that specific channel; otherwise, it joins the voice channel that the user is currently in. It also registers a disconnect handler to clean up resources when the connection is closed.

[!code-cs[Joining Command](Voice/Program.cs#L29-L117)]

The `/leave` command checks if the bot is currently connected to a voice channel in the guild. If connected, it extracts the voice connection, closes the @NetCord.Gateway.Voice.VoiceClient using @NetCord.Gateway.WebSocketClient.CloseAsync*, disposes the voice connection instance, and updates the voice state.

[!code-cs[Leaving Command](Voice/Program.cs#L119-L148)]

That's it! Now your bot can join and leave voice channels. You can test the commands in your Discord server to see them in action. For now, the bot doesn't do anything once it's connected, refer to [Sending Voice](sending-voice.md) and [Receiving Voice](receiving-voice.md) guides to learn how to send and receive audio in voice channels.

## Maintaining Voice Connections

While connecting to a voice channel is straightforward, maintaining a stable connection requires a bit of extra attention. During normal operation, your bot might get moved to another channel by a moderator, or the voice channel's region might get changed.

Discord automatically handles the voice state for your bot, but the underlying voice connection itself may need to be re-established. To detect when a reconnection is necessary, you should listen to the @NetCord.Gateway.GatewayClient.VoiceStateUpdate and @NetCord.Gateway.GatewayClient.VoiceServerUpdate events and verify the bot actually needs to re-establish the connection.

Keep in mind that these events can trigger independently. Because a single event won't provide all the necessary connection details at once, the most robust approach is to combine the fresh data from the newly triggered event with the existing data from your previous voice connection. You can then use this combined data to create a new @NetCord.Gateway.Voice.VoiceClient instance and call @NetCord.Gateway.Voice.VoiceClient.StartAsync*.

The newly created @NetCord.Gateway.Voice.VoiceClient instance is completely independent of the previous one. This means it is up to you to re-apply any ongoing tasks. Any active audio operations or event listeners you had attached to the old client will not carry over automatically and must be explicitly set up again on the new instance.

---

## See Also

- [Sending Voice](sending-voice.md) - Stream audio to voice channels
- [Receiving Voice](receiving-voice.md) - Receive and record voice channel audio
