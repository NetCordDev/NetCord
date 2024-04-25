using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace NetCord.Gateway.Voice.Encryption;

public sealed class Aes256GcmEncryption : IVoiceEncryption
{
    private AesGcm? _encryption;
    private int _nonce;

    private const int TagSize = 16;

    public string Name => "aead_aes256_gcm";

    public int Expansion => TagSize + sizeof(int);

    public bool ExtensionEncryption => true;

    public void Decrypt(RtpPacket packet, Span<byte> plaintext)
    {
        var payload = packet.Payload;

        var noncePart = payload[^sizeof(int)..];

        Span<byte> nonce = stackalloc byte[12];
        ref var nonceRef = ref MemoryMarshal.GetReference(nonce);

        Unsafe.CopyBlockUnaligned(ref nonceRef, ref MemoryMarshal.GetReference(noncePart), sizeof(int));

        var ciphertext = payload[..^(TagSize + sizeof(int))];
        var tag = payload[^(TagSize + sizeof(int))..^sizeof(int)];
        var ad = packet.Header;

        _encryption!.Decrypt(nonce, ciphertext, tag, plaintext, ad);
    }

    public void Encrypt(ReadOnlySpan<byte> plaintext, RtpPacketWriter packet)
    {
        var payload = packet.Payload;

        var noncePart = payload[^sizeof(int)..];

        int nonceValue = Interlocked.Increment(ref _nonce);

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(noncePart), nonceValue);

        Span<byte> nonce = stackalloc byte[12];
        ref var nonceRef = ref MemoryMarshal.GetReference(nonce);

        Unsafe.WriteUnaligned(ref nonceRef, nonceValue);

        var ciphertext = payload[..^(TagSize + sizeof(int))];
        var tag = payload[^(TagSize + sizeof(int))..^sizeof(int)];
        var ad = packet.Header;

        _encryption!.Encrypt(nonce, plaintext, ciphertext, tag, ad);
    }

    public void SetKey(byte[] key)
    {
        _encryption = new(key, TagSize);
    }

    public void Dispose()
    {
        _encryption?.Dispose();
    }
}
