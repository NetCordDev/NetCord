---
title: Discord Voice Integration Guide - Audio Playback and Recording
description: Complete guide to Discord voice in NetCord. Learn voice connections, audio streaming, recording, and real-time voice processing in C#.
omitAppTitle: true
keywords: Discord, voice, audio, voice channels, streaming, recording, music bot, C#, .NET
section: Voice
published_time: '2025-12-15T00:00:00Z'
modified_time: '2026-02-12T00:00:00Z'
---

# Voice Overview

Discord allows you to connect to voice channels and stream audio in real-time. With NetCord, you can create powerful voice bots that can play music, record conversations, and process voice data. This guide covers the essentials of working with Discord voice channels, including connecting, streaming audio and recording.

## Resulting Project

By the end of this guide (the whole voice section), you will have a fully functional music bot that can join voice channels, play audio, and record voice conversations. Of course you can skip for example [Voice Recording](receiving-voice.md), then your bot won't be able to record. Same for [Voice Playback](sending-voice.md). You likely don't want to skip [Voice Connection](voice-connection.md), as without it you won't be able to connect to voice channels, even if you implement the playback and recording.

Below you can see the setup of the project. It will be needed in the following guides of this section. Of course you don't need to follow the guides exactly, the project is just a demonstration of how to use the library, you can implement the features in your own way. The important part is that you understand the concepts and how to use the library to achieve your goals.

#### Program.cs
[!code-cs[Program.cs](VoiceSetup/Program.cs#L3-L30)]

#### VoiceInstance.cs
[!code-cs[VoiceInstance.cs](Voice/VoiceInstance.cs)]

## Glossary

- **PCM**: Pulse-code modulation, a raw audio format.
- **Opus**: An audio codec used by Discord for voice communication. It provides high-quality audio at low bitrates allowing for efficient streaming.
- **Bitrate**: The amount of data transmitted per second in an audio stream. Note that bitrate is not the same as audio quality, as it also depends on the codec and other factors. For example, lossless codecs preserve the original audio at a bitrate that is lower than the bitrate of PCM.

## Terms of Service Compliance

When working with Discord voice, ensure your bot complies with [Discord's Terms of Service](https://discord.com/terms). It is quite easy to violate the terms when working with voice, so be sure to review the guidelines and best practices for voice bots.

---

## See Also

- [Connecting to Voice](voice-connection.md) - Voice connection lifecycle and management
- [Sending Voice](sending-voice.md) - Stream audio to voice channels
- [Receiving Voice](receiving-voice.md) - Receive voice from users
- [Gateway Intents](../events/intents.md) - Configure required intents
- [Discord Docs: Voice Connections](https://discord.com/developers/docs/topics/voice-connections)
