using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace NetCord.Gateway.Voice.Encryption;

public sealed class XSalsa20Poly1305SuffixEncryption : IVoiceEncryption
{
    private byte[]? _key;

    public string Name => "xsalsa20_poly1305_suffix";

    public int Expansion => XSalsa20Poly1305.MacBytes + XSalsa20Poly1305.NonceBytes;

    public bool ExtensionEncryption => true;

    public void Decrypt(RtpPacket packet, Span<byte> plaintext)
    {
        var payload = packet.Payload;

        var nonce = payload[^XSalsa20Poly1305.NonceBytes..];

        var ciphertext = payload[..^XSalsa20Poly1305.NonceBytes];

        int result = XSalsa20Poly1305.CryptoSecretboxOpenEasy(ref MemoryMarshal.GetReference(plaintext),
                                                              ref MemoryMarshal.GetReference(ciphertext),
                                                              (ulong)ciphertext.Length,
                                                              ref MemoryMarshal.GetReference(nonce),
                                                              ref MemoryMarshal.GetArrayDataReference(_key!));

        if (result != 0)
            throw new LibsodiumException();
    }

    public void Encrypt(ReadOnlySpan<byte> plaintext, RtpPacketWriter packet)
    {
        var payload = packet.Payload;

        var nonce = payload[^XSalsa20Poly1305.NonceBytes..];
        RandomNumberGenerator.Fill(nonce);

        var ciphertext = payload[..^XSalsa20Poly1305.NonceBytes];

        int result = XSalsa20Poly1305.CryptoSecretboxEasy(ref MemoryMarshal.GetReference(ciphertext),
                                                          ref MemoryMarshal.GetReference(plaintext),
                                                          (ulong)plaintext.Length,
                                                          ref MemoryMarshal.GetReference(nonce),
                                                          ref MemoryMarshal.GetArrayDataReference(_key!));

        if (result != 0)
            throw new LibsodiumException();
    }

    public void SetKey(byte[] key)
    {
        _key = key;
    }

    public void Dispose()
    {
    }
}
