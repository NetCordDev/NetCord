using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

internal static class Libsodium
{
    /// <param name="c">Encrypted message, length: <paramref name="mlen"/> + 16</param>
    /// <param name="m">Message to encrypt</param>
    /// <param name="mlen">Length of <paramref name="m"/></param>
    /// <param name="n">Nonce, length: 24</param>
    /// <param name="k">Key, length: 32</param>
    /// <returns></returns>
    [DllImport("libsodium", EntryPoint = "crypto_secretbox_easy")]
    public static extern unsafe int CryptoSecretboxEasy(byte* c, byte* m, ulong mlen, byte* n, byte[] k);

    /// <param name="m">Decrypted message, length: <paramref name="clen"/> - 16</param>
    /// <param name="c">Message to decrypt</param>
    /// <param name="clen">Length of <paramref name="c"/></param>
    /// <param name="n">Nonce, length: 24</param>
    /// <param name="k">Key, length: 32</param>
    /// <returns></returns>
    [DllImport("libsodium", EntryPoint = "crypto_secretbox_open_easy")]
    public static extern unsafe int CryptoSecretboxOpenEasy(byte* m, byte* c, ulong clen, byte* n, byte[] k);
}
