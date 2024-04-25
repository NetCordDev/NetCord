using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice.Encryption;

internal static partial class XSalsa20Poly1305
{
    public const int MacBytes = 16;
    public const int NonceBytes = 24;

    [SuppressGCTransition]
    [LibraryImport("libsodium", EntryPoint = "crypto_secretbox_easy")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int CryptoSecretboxEasy(ref byte c, ref byte m, ulong mlen, ref byte n, ref byte k);

    [SuppressGCTransition]
    [LibraryImport("libsodium", EntryPoint = "crypto_secretbox_open_easy")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int CryptoSecretboxOpenEasy(ref byte m, ref byte c, ulong clen, ref byte n, ref byte k);
}

internal static partial class XChaCha20Poly1305
{
    public const int ABytes = 16;
    public const int NPubBytes = 24;

    [SuppressGCTransition]
    [LibraryImport("libsodium", EntryPoint = "crypto_aead_xchacha20poly1305_ietf_encrypt")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int CryptoAeadXChaCha20Poly1305IetfEncrypt(ref byte c, ref ulong clen_p, ref byte m, ulong mlen, ref byte ad, ulong adlen, ref byte nsec, ref byte npub, ref byte k);

    [SuppressGCTransition]
    [LibraryImport("libsodium", EntryPoint = "crypto_aead_xchacha20poly1305_ietf_decrypt")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    internal static partial int CryptoAeadXChaCha20Poly1305IetfDecrypt(ref byte m, ref ulong mlen_p, ref byte nsec, ref byte c, ulong clen, ref byte ad, ulong adlen, ref byte npub, ref byte k);
}
