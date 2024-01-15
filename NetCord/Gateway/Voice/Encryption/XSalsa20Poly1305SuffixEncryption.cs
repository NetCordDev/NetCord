using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace NetCord.Gateway.Voice.Encryption;

public class XSalsa20Poly1305SuffixEncryption : IVoiceEncryption
{
    private byte[]? _key;

    public string Name => "xsalsa20_poly1305_suffix";

    public int Expansion => Libsodium.MacBytes + Libsodium.NonceBytes;

    public void Decrypt(ReadOnlySpan<byte> datagram, Span<byte> plaintext)
    {
        var nonce = datagram[^Libsodium.NonceBytes..];

        var ciphertext = datagram[12..^Libsodium.NonceBytes];

        int result = Libsodium.CryptoSecretboxOpenEasy(ref MemoryMarshal.GetReference(plaintext), ref MemoryMarshal.GetReference(ciphertext), (ulong)ciphertext.Length, ref MemoryMarshal.GetReference(nonce), ref MemoryMarshal.GetArrayDataReference(_key!));

        if (result != 0)
            throw new LibsodiumException();
    }

    public void Encrypt(ReadOnlySpan<byte> plaintext, Span<byte> datagram)
    {
        var nonce = datagram[^Libsodium.NonceBytes..];
        RandomNumberGenerator.Fill(nonce);

        var result = Libsodium.CryptoSecretboxEasy(ref MemoryMarshal.GetReference(datagram[12..]), ref MemoryMarshal.GetReference(plaintext), (ulong)plaintext.Length, ref MemoryMarshal.GetReference(nonce), ref MemoryMarshal.GetArrayDataReference(_key!));

        if (result != 0)
            throw new LibsodiumException();
    }

    public void SetKey(byte[] key)
    {
        _key = key;
    }
}
