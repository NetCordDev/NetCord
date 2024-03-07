using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice.Encryption;

public class XSalsa20Poly1305Encryption : IVoiceEncryption
{
    private byte[]? _key;

    public string Name => "xsalsa20_poly1305";

    public int Expansion => Libsodium.MacBytes;

    public void Decrypt(ReadOnlySpan<byte> datagram, Span<byte> plaintext)
    {
        Span<byte> nonce = stackalloc byte[Libsodium.NonceBytes];
        Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(nonce), ref MemoryMarshal.GetReference(datagram[..12]), 12);

        var cyphertext = datagram[12..];

        int result = Libsodium.CryptoSecretboxOpenEasy(ref MemoryMarshal.GetReference(plaintext),
                                                       ref MemoryMarshal.GetReference(cyphertext),
                                                       (ulong)cyphertext.Length,
                                                       ref MemoryMarshal.GetReference(nonce),
                                                       ref MemoryMarshal.GetArrayDataReference(_key!));

        if (result != 0)
            throw new LibsodiumException();
    }

    public void Encrypt(ReadOnlySpan<byte> plaintext, Span<byte> datagram)
    {
        Span<byte> nonce = stackalloc byte[Libsodium.NonceBytes];
        Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(nonce), ref MemoryMarshal.GetReference(datagram[..12]), 12);

        int result = Libsodium.CryptoSecretboxEasy(ref MemoryMarshal.GetReference(datagram[12..]),
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
}
