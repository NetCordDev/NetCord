namespace NetCord.Gateway.Voice.Streams;

internal class SodiumDecryptStream : RewritingStream
{
    private readonly VoiceClient _client;

    public SodiumDecryptStream(Stream next, VoiceClient client) : base(next)
    {
        _client = client;
    }

    public override unsafe void Write(ReadOnlySpan<byte> buffer)
    {
        Span<byte> nonce = new byte[24];
        buffer[..12].CopyTo(nonce);

        const int headerSize = 12;
        var bufferLen = buffer.Length - headerSize;
        Span<byte> result = new byte[bufferLen - 16];

        int r;
        fixed (byte* resultPtr = result, bufferPtr = buffer, noncePtr = nonce)
            r = Libsodium.CryptoSecretboxOpenEasy(resultPtr, bufferPtr + headerSize, (ulong)bufferLen, noncePtr, _client._secretKey!);

        _next.Write(result[8..]);
    }

    public override unsafe ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var bufferSpan = buffer.Span;
        Span<byte> nonce = new byte[24];
        bufferSpan[..12].CopyTo(nonce);

        const int headerSize = 12;
        var bufferLen = bufferSpan.Length - headerSize;
        Memory<byte> result = new(new byte[bufferLen - 16]);

        int r;
        fixed (byte* resultPtr = result.Span, bufferPtr = bufferSpan, noncePtr = nonce)
            r = Libsodium.CryptoSecretboxOpenEasy(resultPtr, bufferPtr + headerSize, (ulong)bufferLen, noncePtr, _client._secretKey!);

        return _next.WriteAsync(result[8..], cancellationToken);
    }
}