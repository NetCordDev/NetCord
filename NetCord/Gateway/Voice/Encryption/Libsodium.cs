using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice.Encryption;

internal static class Libsodium
{
    public const int KeyBytes = 32;
    public const int MacBytes = 16;
    public const int NonceBytes = 24;

    [DllImport("libsodium", EntryPoint = "crypto_secretbox_easy", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int CryptoSecretboxEasy(ref byte c, ref byte m, ulong mlen, ref byte n, ref byte k);

    [DllImport("libsodium", EntryPoint = "crypto_secretbox_open_easy", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int CryptoSecretboxOpenEasy(ref byte m, ref byte c, ulong clen, ref byte n, ref byte k);
}
