using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice.Encryption;

public class XSalsa20Poly1305LiteEncryption : IVoiceEncryption
{
    private byte[]? _key;
    private int _nonce;

    public string Name => "xsalsa20_poly1305_lite";

    public int Expansion => Libsodium.MacBytes + sizeof(int);

    public void Decrypt(ReadOnlySpan<byte> datagram, Span<byte> plaintext)
    {
        var noncePart = datagram[^sizeof(int)..];
        Span<byte> nonce = stackalloc byte[Libsodium.NonceBytes];
        noncePart.CopyTo(nonce);

        var ciphertext = datagram[12..^sizeof(int)];

        int result = Libsodium.CryptoSecretboxOpenEasy(ref MemoryMarshal.GetReference(plaintext), ref MemoryMarshal.GetReference(ciphertext), (ulong)ciphertext.Length, ref MemoryMarshal.GetReference(nonce), ref MemoryMarshal.GetArrayDataReference(_key!));

        if (result != 0)
            throw new LibsodiumException();
    }

    public void Encrypt(ReadOnlySpan<byte> plaintext, Span<byte> datagram)
    {
        var noncePart = datagram[^sizeof(int)..];
        BinaryPrimitives.WriteInt32BigEndian(noncePart, Interlocked.Increment(ref _nonce));
        Span<byte> nonce = stackalloc byte[Libsodium.NonceBytes];
        noncePart.CopyTo(nonce);

        var result = Libsodium.CryptoSecretboxEasy(ref MemoryMarshal.GetReference(datagram[12..]), ref MemoryMarshal.GetReference(plaintext), (ulong)plaintext.Length, ref MemoryMarshal.GetReference(nonce), ref MemoryMarshal.GetArrayDataReference(_key!));

        if (result != 0)
            throw new LibsodiumException();
    }

    public void SetKey(byte[] key)
    {
        _key = key;
    }
}
