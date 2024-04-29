using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice.Encryption;

public sealed class XSalsa20Poly1305LiteEncryption : IVoiceEncryption
{
    private byte[]? _key;
    private int _nonce;

    public string Name => "xsalsa20_poly1305_lite";

    public int Expansion => XSalsa20Poly1305.MacBytes + sizeof(int);

    public bool ExtensionEncryption => true;

    public void Decrypt(RtpPacket packet, Span<byte> plaintext)
    {
        var payload = packet.Payload;

        var noncePart = payload[^sizeof(int)..];

        Span<byte> nonce = stackalloc byte[XSalsa20Poly1305.NonceBytes];
        ref var nonceRef = ref MemoryMarshal.GetReference(nonce);

        Unsafe.CopyBlockUnaligned(ref nonceRef, ref MemoryMarshal.GetReference(noncePart), sizeof(int));

        var ciphertext = payload[..^sizeof(int)];

        int result = XSalsa20Poly1305.CryptoSecretboxOpenEasy(ref MemoryMarshal.GetReference(plaintext),
                                                              ref MemoryMarshal.GetReference(ciphertext),
                                                              (ulong)ciphertext.Length,
                                                              ref nonceRef,
                                                              ref MemoryMarshal.GetArrayDataReference(_key!));

        if (result != 0)
            throw new LibsodiumException();
    }

    public void Encrypt(ReadOnlySpan<byte> plaintext, RtpPacketWriter packet)
    {
        var payload = packet.Payload;

        var noncePart = payload[^sizeof(int)..];

        int nonceValue = Interlocked.Increment(ref _nonce);

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(noncePart), nonceValue);

        Span<byte> nonce = stackalloc byte[XSalsa20Poly1305.NonceBytes];
        ref var nonceRef = ref MemoryMarshal.GetReference(nonce);

        Unsafe.WriteUnaligned(ref nonceRef, nonceValue);

        var ciphertext = payload[..^sizeof(int)];

        int result = XSalsa20Poly1305.CryptoSecretboxEasy(ref MemoryMarshal.GetReference(ciphertext),
                                                          ref MemoryMarshal.GetReference(plaintext),
                                                          (ulong)plaintext.Length,
                                                          ref nonceRef,
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
