# Voice

## Installation

# [Windows](#tab/installation-windows)
Sending and receiving voice requires `opus.dll` and `libsodium.dll` files. You should place them in your application runtime directory.

# [Linux and MacOS](#tab/installation-linux-and-macos)
Sending and receiving voice requires `opus.so` and `libsodium.so` files. You should place them in your application runtime directory or install them using a package manager.

***

## Connecting
You can join a voice channel using @NetCord.Gateway.GatewayClient.UpdateVoiceStateAsync(NetCord.Gateway.VoiceStateProperties). When you use it, if you are successfully connected to the voice channel, you will receive @NetCord.Gateway.GatewayClient.VoiceStateUpdate event, and then @NetCord.Gateway.GatewayClient.VoiceServerUpdate event. When you receive them, you can create @NetCord.Gateway.Voice.VoiceClient instance and start sending voice! Example:
[!code-cs[VoiceModule.cs](voice/VoiceModule.cs#L12-L56)]

## Example Usage

### Sending Voice
[!code-cs[VoiceModule.cs](voice/VoiceModule.cs#L58-L101)]

### Receiving Voice
> [!NOTE]
> To receive voice, you need to set `RedirectInputStreams` to `true` in configuration, add `new VoiceClientConfiguration() { RedirectInputStreams = true }` as a last parameter when creating @NetCord.Gateway.Voice.VoiceClient instance.

[!code-cs[VoiceModule.cs](voice/VoiceModule.cs#L103-L115)]

#### Detecting Voice Author
You can use @NetCord.Gateway.Voice.VoiceClient.Users to get user id using ssrc from @NetCord.Gateway.Voice.VoiceClient.VoiceReceive event.

## Different Types of Streams
A difference between a stream from @NetCord.Gateway.Voice.VoiceClient.CreatePCMStream(NetCord.Gateway.Voice.OpusApplication) and a stream from @NetCord.Gateway.Voice.VoiceClient.CreateDirectPCMStream(NetCord.Gateway.Voice.OpusApplication) is that the first will automatically normalize voice sending speed.