using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NetCord.Rest;

public class HttpInteractionValidator
{
    private readonly byte[] _publicKey;

    public HttpInteractionValidator(ReadOnlySpan<char> publicKey)
    {
        if (!ValidatePublicKey(publicKey))
            throw new ArgumentException($"'{nameof(publicKey)}' must be 32 bytes long.", nameof(publicKey));

        _publicKey = Convert.FromHexString(publicKey);
    }

    public HttpInteractionValidator(byte[] publicKey)
    {
        if (!ValidatePublicKey(publicKey))
            throw new ArgumentException($"'{nameof(publicKey)}' must be 32 bytes long.", nameof(publicKey));

        _publicKey = publicKey;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signature">The value of <c>X-Signature-Ed25519</c> header.</param>
    /// <param name="timestamp">The value of <c>X-Signature-Timestamp</c> header.</param>
    /// <param name="body">The request body.</param>
    /// <returns></returns>
    public bool Validate(ReadOnlySpan<char> signature, ReadOnlySpan<char> timestamp, ReadOnlySpan<byte> body)
    {
        if (!ValidateSignature(signature))
            return false;

        Span<byte> signatureBytes = stackalloc byte[64];
        if (!TryDecodeFromUtf16(signature, signatureBytes))
            return false;

        int timestampLength = Encoding.UTF8.GetByteCount(timestamp);
        int timestampAndBodyLength = timestampLength + body.Length;

        byte[]? toReturn = null;
        Span<byte> timestampAndBody = timestampAndBodyLength <= 256 ? stackalloc byte[256] : (toReturn = ArrayPool<byte>.Shared.Rent(timestampAndBodyLength));

        Encoding.UTF8.GetBytes(timestamp, timestampAndBody);
        body.CopyTo(timestampAndBody[timestampLength..]);

        var result = ValidateUnsafe(signatureBytes, timestampAndBody[..timestampAndBodyLength]);

        if (toReturn is not null)
            ArrayPool<byte>.Shared.Return(toReturn);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signature">The value of <c>X-Signature-Ed25519</c> header.</param>
    /// <param name="timestampAndBody">he value of <c>X-Signature-Timestamp</c> header as bytes appended by the request body.</param>
    /// <returns></returns>
    public bool Validate(ReadOnlySpan<char> signature, ReadOnlySpan<byte> timestampAndBody)
    {
        if (!ValidateSignature(signature))
            return false;

        Span<byte> signatureBytes = stackalloc byte[64];
        if (!TryDecodeFromUtf16(signature, signatureBytes))
            return false;

        return ValidateUnsafe(signatureBytes, timestampAndBody);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signature">The hex decoded value of <c>X-Signature-Ed25519</c> header.</param>
    /// <param name="timestamp">The value of <c>X-Signature-Timestamp</c> header as bytes.</param>
    /// <param name="body">The request body.</param>
    /// <returns></returns>
    public bool Validate(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> timestamp, ReadOnlySpan<byte> body)
    {
        if (!ValidateSignature(signature))
            return false;

        int timestampLength = timestamp.Length;
        int timestampAndBodyLength = timestampLength + body.Length;

        byte[]? toReturn = null;
        Span<byte> timestampAndBody = timestampAndBodyLength <= 256 ? stackalloc byte[256] : (toReturn = ArrayPool<byte>.Shared.Rent(timestampAndBodyLength));

        timestamp.CopyTo(timestampAndBody);
        body.CopyTo(timestampAndBody[timestampLength..]);

        var result = ValidateUnsafe(signature, timestampAndBody[..timestampAndBodyLength]);

        if (toReturn is not null)
            ArrayPool<byte>.Shared.Return(toReturn);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signature">The hex decoded value of <c>X-Signature-Ed25519</c> header.</param>
    /// <param name="timestampAndBody">The value of <c>X-Signature-Timestamp</c> header as bytes appended by the request body.</param>
    /// <returns></returns>
    public bool Validate(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> timestampAndBody)
    {
        if (!ValidateSignature(signature))
            return false;

        return ValidateUnsafe(signature, timestampAndBody);
    }

    private bool ValidateUnsafe(ReadOnlySpan<byte> signature, ReadOnlySpan<byte> timestampAndBody)
    {
        int result = CryptoSignEd25519VerifyDetached(ref MemoryMarshal.GetReference(signature!),
                                                     ref MemoryMarshal.GetReference(timestampAndBody),
                                                     (ulong)timestampAndBody.Length,
                                                     ref MemoryMarshal.GetArrayDataReference(_publicKey));

        return result is 0;
    }

    private static bool ValidatePublicKey(ReadOnlySpan<char> publicKey) => publicKey.Length is 64;

    private static bool ValidatePublicKey(byte[] publicKey) => publicKey.Length is 32;

    private static bool ValidateSignature(ReadOnlySpan<char> signature) => signature.Length is 128;

    private static bool ValidateSignature(ReadOnlySpan<byte> signature) => signature.Length is 64;

    [DllImport("libsodium", EntryPoint = "crypto_sign_ed25519_verify_detached", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CryptoSignEd25519VerifyDetached(ref byte sig, ref byte m, ulong mlen, ref byte pk);

    #region From https://github.com/dotnet/runtime/blob/release/6.0/src/libraries/Common/src/System/HexConverter.cs
    private static bool TryDecodeFromUtf16(ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        return TryDecodeFromUtf16(chars, bytes, out _);
    }

    private static bool TryDecodeFromUtf16(ReadOnlySpan<char> chars, Span<byte> bytes, out int charsProcessed)
    {
        Debug.Assert(chars.Length % 2 == 0, "Un-even number of characters provided");
        Debug.Assert(chars.Length / 2 == bytes.Length, "Target buffer not right-sized for provided characters");

        int i = 0;
        int j = 0;
        int byteLo = 0;
        int byteHi = 0;
        while (j < bytes.Length)
        {
            byteLo = FromChar(chars[i + 1]);
            byteHi = FromChar(chars[i]);

            // byteHi hasn't been shifted to the high half yet, so the only way the bitwise or produces this pattern
            // is if either byteHi or byteLo was not a hex character.
            if ((byteLo | byteHi) == 0xFF)
                break;

            bytes[j++] = (byte)((byteHi << 4) | byteLo);
            i += 2;
        }

        if (byteLo == 0xFF)
            i++;

        charsProcessed = i;
        return (byteLo | byteHi) != 0xFF;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FromChar(int c)
    {
        return c >= CharToHexLookup.Length ? 0xFF : CharToHexLookup[c];
    }

    /// <summary>Map from an ASCII char to its hex value, e.g. arr['b'] == 11. 0xFF means it's not a hex digit.</summary>
    private static ReadOnlySpan<byte> CharToHexLookup => new byte[]
    {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 15
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 31
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 47
            0x0,  0x1,  0x2,  0x3,  0x4,  0x5,  0x6,  0x7,  0x8,  0x9,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 63
            0xFF, 0xA,  0xB,  0xC,  0xD,  0xE,  0xF,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 79
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 95
            0xFF, 0xa,  0xb,  0xc,  0xd,  0xe,  0xf,  0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 111
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 127
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 143
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 159
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 175
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 191
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 207
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 223
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 239
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF  // 255
    };
    #endregion
}
