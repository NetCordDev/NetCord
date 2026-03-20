using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice.Encryption;

internal static partial class XChaCha20Poly1305
{
    private const string DllName = "libsodium";

    public const int ABytes = 16;
    public const int NPubBytes = 24;

    [LibraryImport(DllName, EntryPoint = "crypto_aead_xchacha20poly1305_ietf_encrypt")]
    internal static partial int CryptoAeadXChaCha20Poly1305IetfEncrypt(ref byte c, ref ulong clen_p, ref byte m, ulong mlen, ref byte ad, ulong adlen, ref byte nsec, ref byte npub, ref byte k);

    [LibraryImport(DllName, EntryPoint = "crypto_aead_xchacha20poly1305_ietf_decrypt")]
    internal static partial int CryptoAeadXChaCha20Poly1305IetfDecrypt(ref byte m, ref ulong mlen_p, ref byte nsec, ref byte c, ulong clen, ref byte ad, ulong adlen, ref byte npub, ref byte k);
}
