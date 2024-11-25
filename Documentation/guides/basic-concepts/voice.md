# Voice

## Native Dependencies

Follow the [installation guide](installing-native-dependencies.md) to install the required native dependencies.

## Example Usage

> [!NOTE]
> In the following examples streams and @NetCord.Gateway.Voice.VoiceClient instances are not disposed because they should be stored somewhere and disposed later.

### Sending Voice
[!code-cs[VoiceModule.cs](Voice/VoiceModule.cs#L12-L105)]

### Receiving Voice
[!code-cs[VoiceModule.cs](Voice/VoiceModule.cs#L107-L146)]
