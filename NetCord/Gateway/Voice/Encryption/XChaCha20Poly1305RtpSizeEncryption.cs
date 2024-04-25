using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice.Encryption;

public sealed class XChaCha20Poly1305RtpSizeEncryption : IVoiceEncryption
{
    private byte[]? _key;
    private int _nonce;

    public string Name => "aead_xchacha20_poly1305_rtpsize";

    public int Expansion => XChaCha20Poly1305.ABytes + sizeof(int);

    public bool ExtensionEncryption => false;

    public void Decrypt(RtpPacket packet, Span<byte> plaintext)
    {
        var payload = packet.Payload;

        var noncePart = payload[^sizeof(int)..];

        Span<byte> nonce = stackalloc byte[XChaCha20Poly1305.NPubBytes];
        ref var nonceRef = ref MemoryMarshal.GetReference(nonce);

        Unsafe.CopyBlockUnaligned(ref nonceRef, ref MemoryMarshal.GetReference(noncePart), sizeof(int));

        var ciphertext = payload[..^sizeof(int)];
        var ad = packet.ExtendedHeader;

        int result = XChaCha20Poly1305.CryptoAeadXChaCha20Poly1305IetfDecrypt(ref MemoryMarshal.GetReference(plaintext),
                                                                              ref Unsafe.NullRef<ulong>(),
                                                                              ref Unsafe.NullRef<byte>(),
                                                                              ref MemoryMarshal.GetReference(ciphertext),
                                                                              (ulong)ciphertext.Length,
                                                                              ref MemoryMarshal.GetReference(ad),
                                                                              (ulong)ad.Length,
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

        Span<byte> nonce = stackalloc byte[XChaCha20Poly1305.NPubBytes];
        ref var nonceRef = ref MemoryMarshal.GetReference(nonce);

        Unsafe.WriteUnaligned(ref nonceRef, nonceValue);

        var ciphertext = payload[..^sizeof(int)];
        var ad = packet.ExtendedHeader;

        int result = XChaCha20Poly1305.CryptoAeadXChaCha20Poly1305IetfEncrypt(ref MemoryMarshal.GetReference(ciphertext),
                                                                              ref Unsafe.NullRef<ulong>(),
                                                                              ref MemoryMarshal.GetReference(plaintext),
                                                                              (ulong)plaintext.Length,
                                                                              ref MemoryMarshal.GetReference(ad),
                                                                              (ulong)ad.Length,
                                                                              ref Unsafe.NullRef<byte>(),
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
