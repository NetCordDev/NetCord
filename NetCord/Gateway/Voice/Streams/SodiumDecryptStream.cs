using System.Buffers;

namespace NetCord.Gateway.Voice.Streams;

internal class SodiumDecryptStream : RewritingStream
{
    private readonly VoiceClient _client;

    public SodiumDecryptStream(Stream next, VoiceClient client) : base(next)
    {
        _client = client;
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        int bufferLen = buffer.Length - 12;
        int resultLen = bufferLen - 16;
        using var owner = MemoryPool<byte>.Shared.Rent(resultLen);
        var result = owner.Memory[..resultLen];
        Decrypt(result.Span, buffer.Span, bufferLen);
        await _next.WriteAsync(result[8..], cancellationToken).ConfigureAwait(false);
    }

    public override unsafe void Write(ReadOnlySpan<byte> buffer)
    {
        int bufferLen = buffer.Length - 12;
        int resultLen = bufferLen - 16;
        using var owner = MemoryPool<byte>.Shared.Rent(resultLen);
        var result = owner.Memory.Span[..resultLen];
        Decrypt(result, buffer, bufferLen);
        _next.Write(result[8..]);
    }

    private unsafe void Decrypt(Span<byte> result, ReadOnlySpan<byte> buffer, int bufferLen)
    {
        var noncePtr = stackalloc byte[24];
        Span<byte> nonce = new(noncePtr, 24);
        buffer[..12].CopyTo(nonce);

        fixed (byte* resultPtr = result, bufferPtr = buffer)
            _ = Libsodium.CryptoSecretboxOpenEasy(resultPtr, bufferPtr + 12, (ulong)bufferLen, noncePtr, _client._secretKey!);
    }
}
