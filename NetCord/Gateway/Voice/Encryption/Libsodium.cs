using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice.Encryption;

internal static partial class Libsodium
{
    public const int KeyBytes = 32;
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
