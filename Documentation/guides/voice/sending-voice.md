---
title: Sending Audio to Discord Voice Channels with NetCord
description: Stream audio to Discord voice channels in C#. Learn audio encoding, streaming, format conversion, and music bot development with NetCord.
omitAppTitle: true
keywords: Discord, audio, audio streaming, Opus encoding, music bot, voice audio, audio playback, C#, .NET
section: Voice
published_time: '2025-12-15T00:00:00Z'
modified_time: '2025-02-12T00:00:00Z'
---

# Sending Voice

In this section, you'll learn how to send audio to Discord voice channels using NetCord. We'll cover the basics of audio streaming, encoding, and best practices for creating music bots and other voice applications.

## Sending Audio

NetCord provides 2 ways of sending audio to voice channels. @NetCord.Gateway.Voice.VoiceClient.CreateVoiceStream* method creates a stream that you can write Opus-encoded audio data to, and it will handle sending it to Discord. If you need more control over the streaming process, you can use @NetCord.Gateway.Voice.VoiceClient.SendVoice* or @NetCord.Gateway.Voice.VoiceClient.SendVoiceAsync*. They allow you to send Opus-encoded audio data directly as well as handling the timing of the packets.

For most music bots, using the stream returned by @NetCord.Gateway.Voice.VoiceClient.CreateVoiceStream* is sufficient. It's easier to use and abstracts away the complexities of packet timing and management. It also integrates well with @NetCord.Gateway.Voice.OpusEncodeStream, allowing you to easily encode and stream audio in one step, which you will learn about in the next section.

[!code-cs[Creating Voice Stream](SendingVoice/Examples.cs#L11)]

The voice stream also supports configuration options, such as disabling the speed normalization or using a custom frame duration, you can set these options using @NetCord.Gateway.Voice.VoiceStreamConfiguration when creating the stream. Note that disabling speed normalization makes you responsible for ensuring that the audio is written at the correct speed. Frame duration is also important to get right, you must ensure that the Opus frames you write match the frame duration specified in the configuration, otherwise you may encounter issues with audio timing. Allowed frame durations in milliseconds are 2.5, 5, 10, 20, 40, 60, 80, 100, and 120 (120 ms is only supported for mono audio). The default frame duration is 20ms. Refer to the [Opus Documentation](https://wiki.xiph.org/Opus_Recommended_Settings#Framesize_Tweaking) for more details on frame duration and its impact.

[!code-cs[Voice Stream Configuration](SendingVoice/Examples.cs#L16-L20)]

## Opus Encoding

Discord uses the Opus codec for voice communication. To send audio to a voice channel, you need to encode your audio data in Opus format. NetCord provides @NetCord.Gateway.Voice.OpusEncodeStream, which allows you to encode PCM audio data into Opus format that can be sent to Discord. It uses @NetCord.Gateway.Voice.OpusEncoder internally, which can be used directly if you need more control over the encoding process.

When you create an @NetCord.Gateway.Voice.OpusEncodeStream, you need to specify the PCM format, the number of channels, as well as the Opus application. The supported PCM formats are 16-bit signed integer (@"NetCord.Gateway.Voice.PcmFormat.Short") and 32-bit floating point (@"NetCord.Gateway.Voice.PcmFormat.Float"). @NetCord.Gateway.Voice.PcmFormat.Float is generally preferred for high-quality audio. The number of channels can be either 1 (@"NetCord.Gateway.Voice.VoiceChannels.Mono") or 2 (@"NetCord.Gateway.Voice.VoiceChannels.Stereo"). The Opus application can be either @NetCord.Gateway.Voice.OpusApplication.Voip, @NetCord.Gateway.Voice.OpusApplication.Audio, @NetCord.Gateway.Voice.OpusApplication.RestrictedLowdelay, @NetCord.Gateway.Voice.OpusApplication.RestrictedSilk, or @NetCord.Gateway.Voice.OpusApplication.RestrictedCelt, depending on your use case. For music bots, you should use @NetCord.Gateway.Voice.OpusApplication.Audio, as it is optimized for music streaming. Feel free to refer to @NetCord.Gateway.Voice.OpusApplication for more details on the different Opus applications and their use cases.

[!code-cs[Creating Opus Encode Stream](SendingVoice/Examples.cs#L25-L28)]

Aside of the required parameters, you can also specify whether to segment the audio and the frame duration to use when encoding. If you choose to segment the audio, the stream will buffer the PCM data until it has enough to encode a full Opus frame. If you choose not to segment, you need to ensure that you write PCM data in chunks that correspond to the Opus frame size. Segmentation is enabled by default. You can also specify the frame duration to use when encoding, which must match the frame duration used by the voice stream you are writing to. The default frame duration is 20ms and matches the default frame duration of the voice stream.

[!code-cs[Opus Encode Stream Configuration](SendingVoice/Examples.cs#L33-L41)]

## Audio Formats

Discord voice channels only support raw Opus-encoded audio. Since Opus is a codec and not a container format, you will need to convert your audio files to raw Opus format before sending them to a voice channel. This typically involves converting your audio data to PCM and then encoding it to Opus format using @NetCord.Gateway.Voice.OpusEncodeStream or @NetCord.Gateway.Voice.OpusEncoder.

You can use for example [FFmpeg](https://ffmpeg.org) to convert various audio formats to PCM. The example below demonstrates how to use it to convert an input audio to 32-bit float PCM format and 2 channels and then stream the audio directly to an @NetCord.Gateway.Voice.OpusEncodeStream, which will handle encoding it to Opus and pass it to the voice stream to be sent to Discord.

[!code-cs[Using FFmpeg to Send Audio](SendingVoice/Examples.cs#L46-L72)]

## Updating Our Bot

Now, it's time to finally... add a `/play` command to our bot! This command plays an audio file in the voice channel the bot is currently connected to.

[!code-cs[Play Command](Voice/Program.cs#L150-L221)]

Now, our bot can finally play audio files! You can test it out by using the `/play` command. Please note that you need to use the `/join` command to have the bot join a voice channel before you can use the `/play` command.

---

## See Also

- [Voice Connection](voice-connection.md) - Join and manage voice channels
- [Receiving Voice](receiving-voice.md) - Receive and record voice
