# Voice

## Installation

# [Windows](#tab/installation-windows)
Sending and receiving voice requires `libsodium.dll` and optionally `opus.dll` files. You should place them in the runtime directory of your application.

# [Linux and MacOS](#tab/installation-linux-and-macos)
Sending and receiving voice requires `libsodium.so` and optionally `opus.so` files. You should place them in the runtime directory of your application or install them using a package manager.

***

## Connecting
You can join a voice channel using @NetCord.Gateway.GatewayClient.UpdateVoiceStateAsync(NetCord.Gateway.VoiceStateProperties). When you use it, if you are successfully connected to the voice channel, you will receive @NetCord.Gateway.GatewayClient.VoiceStateUpdate event, and then @NetCord.Gateway.GatewayClient.VoiceServerUpdate event. When you receive them, you can create @NetCord.Gateway.Voice.VoiceClient instance and start sending voice! Example:
[!code-cs[VoiceModule.cs](voice/VoiceModule.cs#L12-L83)]

## Example Usage

> [!NOTE]
> In the following examples streams and @NetCord.Gateway.Voice.VoiceClient instances are not disposed because they should be stored somewhere and disposed later.

### Sending Voice
[!code-cs[VoiceModule.cs](voice/VoiceModule.cs#L85-L159)]

### Receiving Voice
[!code-cs[VoiceModule.cs](voice/VoiceModule.cs#L161-L187)]