---
title: Receiving Audio from Discord Voice Channels with NetCord
description: Receive voice channel audio in C#. Learn audio capture, decoding, user audio streams, and voice processing with NetCord.
omitAppTitle: true
keywords: Discord, voice audio, audio capture, audio decoding, voice recording, audio processing, user streams, C#, .NET
section: Voice
published_time: '2025-12-15T00:00:00Z'
modified_time: '2025-02-15T00:00:00Z'
---

# Receiving Voice

This section will cover how to receive audio from Discord voice channels using NetCord. You'll learn how to capture audio from users in a voice channel, decode the Opus-encoded audio data, and process it for various applications such as recording.

## Receiving Audio

Receiving audio from @NetCord.Gateway.Voice.VoiceClient is done through the @NetCord.Gateway.Voice.VoiceClient.VoiceReceive event. This event is triggered whenever a new Opus-encoded audio packet is received from Discord. The event handler receives a @NetCord.Gateway.Voice.VoiceReceiveEventArgs object, which contains:
- @NetCord.Gateway.Voice.VoiceReceiveEventArgs.Frame: The Opus-encoded audio data received.
- @NetCord.Gateway.Voice.VoiceReceiveEventArgs.Ssrc: The SSRC of the user who sent the audio data.
- @NetCord.Gateway.Voice.VoiceReceiveEventArgs.Timestamp: The timestamp of the received audio packet. This is useful for jitter buffering and synchronization.
- @NetCord.Gateway.Voice.VoiceReceiveEventArgs.SequenceNumber: The sequence number of the received audio packet.

The @NetCord.Gateway.Voice.VoiceReceiveEventArgs does not contain any user information, only the SSRC. You can use the SSRC to identify which user sent the audio data by looking it up in the @NetCord.Gateway.Voice.VoiceClient's cache. See the example below for how to do that.

[!code-cs[Voice Receive Event](ReceivingVoice/Examples.cs#L11-L19)]

You may notice that the @NetCord.Gateway.Voice.VoiceReceiveEventArgs is a `ref struct`. This is because it uses an internal buffer used by NetCord to receive audio data from Discord. You don't need to worry about managing this buffer carefully, because the compiler ensures that you don't accidentally store a reference to it or use it after the event handler returns. This also comes with some limitations. Handlers of this event cannot be `async`. If you need to perform asynchronous operations with the received audio data, you can copy the Opus frame data to a separate buffer and then process it asynchronously.

[!code-cs[Processing Received Audio Asynchronously](ReceivingVoice/Examples.cs#L24-L38)]

Also keep in mind that the @NetCord.Gateway.Voice.VoiceClient.VoiceReceive event is raised for every received audio packet, which can be quite frequent. If you perform heavy (synchronous) processing in the event handler, it can cause your bot to stop keeping up with the incoming audio data. Consider using @System.Threading.Channels.Channel<T> to buffer the received audio data and process it in a separate task to avoid this issue.

[!code-cs[Buffering Received Audio](ReceivingVoice/Examples.cs#L43-L64)]

## Opus Decoding

Discord sends audio in Opus format, which you may need to decode to PCM for processing. NetCord provides @NetCord.Gateway.Voice.OpusDecodeStream, which decodes Opus-encoded audio data that gets written into PCM format and writes the PCM data to the stream passed into its constructor. It uses @NetCord.Gateway.Voice.OpusDecoder internally, which can be used directly if you need more control over the decoding process.

When creating the @NetCord.Gateway.Voice.OpusDecodeStream you need to specify the PCM format and the number of channels in addition to the stream to write the decoded audio data to. The supported PCM formats are 16-bit signed integer (@"NetCord.Gateway.Voice.PcmFormat.Short") and 32-bit floating point (@"NetCord.Gateway.Voice.PcmFormat.Float"). @NetCord.Gateway.Voice.PcmFormat.Float is generally preferred for high-quality audio. The number of channels can be either 1 (@"NetCord.Gateway.Voice.VoiceChannels.Mono") or 2 (@"NetCord.Gateway.Voice.VoiceChannels.Stereo").

[!code-cs[Creating OpusDecodeStream](ReceivingVoice/Examples.cs#L69-L71)]

## Updating Our Bot

Now it's time to add a `/record` command to our bot that records what a user is saying in a voice channel and sends it to the channel where the command was used, either when a max file size is triggered or when the bot leaves the voice channel. This command will use the knowledge we have gained about receiving audio to capture the user's audio and save it to a file.

[!code-cs[Record Command](Voice/Program.cs#L222-L358)]

---

## See Also

- [Voice Connection](voice-connection.md) - Join and manage voice channels
- [Sending Voice](sending-voice.md) - Send audio to voice channels
