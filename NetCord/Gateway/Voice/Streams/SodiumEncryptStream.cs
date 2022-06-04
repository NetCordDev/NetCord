using System.Buffers.Binary;

namespace NetCord.Gateway.Voice.Streams;

internal class SodiumEncryptStream : RewritingStream
{
    private readonly VoiceClient _client;

    public SodiumEncryptStream(Stream next, VoiceClient client) : base(next)
    {
        _client = client;
    }

    public unsafe override void Write(ReadOnlySpan<byte> buffer)
    {
        var bufferLen = buffer.Length;
        Span<byte> result = new(new byte[bufferLen + 16 + 12]);

        var nonce = CreateNonce();
        nonce[..12].CopyTo(result);

        int r;
        fixed (byte* resultPtr = result, bufferPtr = buffer, noncePtr = nonce)
            r = Libsodium.CryptoSecretboxEasy(resultPtr + 12, bufferPtr, (ulong)bufferLen, noncePtr, _client._secretKey!);

        _next.Write(result);
    }

    public unsafe override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var bufferLen = buffer.Length;
        Memory<byte> result = new(new byte[bufferLen + 16 + 12]);
        var resultSpan = result.Span;

        var nonce = CreateNonce();
        nonce[..12].CopyTo(resultSpan);

        int r;
        fixed (byte* resultPtr = resultSpan, bufferPtr = buffer.Span, noncePtr = nonce)
            r = Libsodium.CryptoSecretboxEasy(resultPtr + 12, bufferPtr, (ulong)bufferLen, noncePtr, _client._secretKey!);

        return _next.WriteAsync(result, cancellationToken);
    }

    private Span<byte> CreateNonce()
    {
        Span<byte> nonce = new byte[24];
        nonce[0] = 0x80;
        nonce[1] = 0x78;
        BinaryPrimitives.WriteUInt16BigEndian(nonce[2..], _client._sequenceNumber++);
        BinaryPrimitives.WriteUInt32BigEndian(nonce[4..], _client._timestamp);
        _client._timestamp += Opus.FrameSamplesPerChannel;
        BinaryPrimitives.WriteUInt32BigEndian(nonce[8..], _client.Ssrc);
        return nonce;
    }
}
