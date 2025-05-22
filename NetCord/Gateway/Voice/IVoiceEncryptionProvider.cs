using NetCord.Gateway.Voice.Encryption;

namespace NetCord.Gateway.Voice;

public interface IVoiceEncryptionProvider
{
    public IVoiceEncryption GetEncryption(IReadOnlyList<string> modes);
}
