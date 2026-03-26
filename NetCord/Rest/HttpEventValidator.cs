using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

namespace NetCord.Rest;

public partial class HttpEventValidator
{
    private readonly byte[] _publicKey;

    public HttpEventValidator(ReadOnlySpan<char> publicKey)
    {
        if (!ValidatePublicKey(publicKey))
            throw new ArgumentException($"'{nameof(publicKey)}' must be 32 bytes long.", nameof(publicKey));

        _publicKey = Convert.FromHexString(publicKey);
    }

    public HttpEventValidator(byte[] publicKey)
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
        if (!DecodeSignature(signature, signatureBytes))
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
        if (!DecodeSignature(signature, signatureBytes))
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

    private static bool DecodeSignature(ReadOnlySpan<char> chars, Span<byte> bytes)
    {
        return Convert.FromHexString(chars, bytes, out int _, out int _) is OperationStatus.Done;
    }

    [LibraryImport("libsodium", EntryPoint = "crypto_sign_ed25519_verify_detached")]
    private static partial int CryptoSignEd25519VerifyDetached(ref byte sig, ref byte m, ulong mlen, ref byte pk);
}
