using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice.Encryption;

public sealed class XSalsa20Poly1305Encryption : IVoiceEncryption
{
    private byte[]? _key;

    public string Name => "xsalsa20_poly1305";

    public int Expansion => XSalsa20Poly1305.MacBytes;

    public bool ExtensionEncryption => true;

    public void Decrypt(RtpPacket packet, Span<byte> plaintext)
    {
        Span<byte> nonce = stackalloc byte[XSalsa20Poly1305.NonceBytes];
        Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(nonce), ref MemoryMarshal.GetReference(packet.FixedHeader), 12);

        var ciphertext = packet.Payload;

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
        Span<byte> nonce = stackalloc byte[XSalsa20Poly1305.NonceBytes];
        Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(nonce), ref MemoryMarshal.GetReference(packet.FixedHeader), 12);

        var payload = packet.Payload;

        int result = XSalsa20Poly1305.CryptoSecretboxEasy(ref MemoryMarshal.GetReference(payload),
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
