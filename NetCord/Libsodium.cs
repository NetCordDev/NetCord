using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NetCord;

internal static partial class Libsodium
{
    static Libsodium()
    {
        if (SodiumInit() < 0)
            ThrowFailedToInitialize();
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowFailedToInitialize()
    {
        throw new LibsodiumException("Failed to initialize libsodium.");
    }

    private const string DllName = "libsodium";

    [LibraryImport(DllName, EntryPoint = "sodium_init")]
    private static partial int SodiumInit();

    // XChaCha20-Poly1305
    public const int XChaCha20Poly1305_ABytes = 16;
    public const int XChaCha20Poly1305_NPubBytes = 24;

    [LibraryImport(DllName, EntryPoint = "crypto_aead_xchacha20poly1305_ietf_encrypt")]
    internal static partial int XChaCha20Poly1305_CryptoAeadXChaCha20Poly1305IetfEncrypt(ref byte c,
                                                                                         ref ulong clen_p,
                                                                                         ref byte m,
                                                                                         ulong mlen,
                                                                                         ref byte ad,
                                                                                         ulong adlen,
                                                                                         ref byte nsec,
                                                                                         ref byte npub,
                                                                                         ref byte k);

    [LibraryImport(DllName, EntryPoint = "crypto_aead_xchacha20poly1305_ietf_decrypt")]
    internal static partial int XChaCha20Poly1305_CryptoAeadXChaCha20Poly1305IetfDecrypt(ref byte m,
                                                                                         ref ulong mlen_p,
                                                                                         ref byte nsec,
                                                                                         ref byte c,
                                                                                         ulong clen,
                                                                                         ref byte ad,
                                                                                         ulong adlen,
                                                                                         ref byte npub,
                                                                                         ref byte k);

    // Ed25519
    [LibraryImport(DllName, EntryPoint = "crypto_sign_ed25519_verify_detached")]
    internal static partial int Ed25519_CryptoSignEd25519VerifyDetached(ref byte sig,
                                                                        ref byte m,
                                                                        ulong mlen,
                                                                        ref byte pk);
}
