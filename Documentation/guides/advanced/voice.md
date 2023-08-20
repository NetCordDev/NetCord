# Voice

## Installation

# [Windows](#tab/installation-windows)
Sending and receiving voice requires `libsodium.dll` and optionally `opus.dll` files. You should place them in the runtime directory of your application.

# [Linux and MacOS](#tab/installation-linux-and-macos)
Sending and receiving voice requires `libsodium.so` and optionally `opus.so` files. You should place them in the runtime directory of your application or install them using a package manager.

***

## Example Usage

> [!NOTE]
> In the following examples streams and @NetCord.Gateway.Voice.VoiceClient instances are not disposed because they should be stored somewhere and disposed later.

### Sending Voice
[!code-cs[VoiceModule.cs](voice/VoiceModule.cs#L12-L103)]

### Receiving Voice
[!code-cs[VoiceModule.cs](voice/VoiceModule.cs#L105-L147)]