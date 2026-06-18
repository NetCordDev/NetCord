using NetCord.Gateway.Voice.Encryption;

namespace NetCord.Gateway.Voice;

public class VoiceEncryptionProvider : IVoiceEncryptionProvider
{
    public static VoiceEncryptionProvider Instance { get; } = new();

    private VoiceEncryptionProvider()
    {
    }

    public IVoiceEncryption GetEncryption(IReadOnlyList<string> modes)
    {
        return (modes.Contains("aead_aes256_gcm_rtpsize") && Aes256GcmRtpSizeEncryption.IsSupported)
            ? new Aes256GcmRtpSizeEncryption()
            : new XChaCha20Poly1305RtpSizeEncryption();
    }
}
