using System.Buffers;
using System.Buffers.Binary;

namespace NetCord.Gateway.Voice.Streams;

internal class SodiumEncryptStream : RewritingStream
{
    private readonly VoiceClient _client;

    public SodiumEncryptStream(Stream next, VoiceClient client) : base(next)
    {
        _client = client;
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var bufferLen = buffer.Length;
        int resultLen = bufferLen + 16 + 12;
        using var owner = MemoryPool<byte>.Shared.Rent(resultLen);
        var result = owner.Memory[..resultLen];
        Encrypt(result.Span, buffer.Span, bufferLen);
        await _next.WriteAsync(result, cancellationToken).ConfigureAwait(false);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        int bufferLen = buffer.Length;
        int resultLen = bufferLen + 16 + 12;
        using var owner = MemoryPool<byte>.Shared.Rent(resultLen);
        var result = owner.Memory.Span[..resultLen];
        Encrypt(result, buffer, bufferLen);
        _next.Write(result);
    }

    private unsafe void Encrypt(Span<byte> result, ReadOnlySpan<byte> buffer, int bufferLen)
    {
        var noncePtr = stackalloc byte[24];
        Span<byte> nonce = new(noncePtr, 24);
        nonce[0] = 0x80;
        nonce[1] = 0x78;
        BinaryPrimitives.WriteUInt16BigEndian(nonce[2..], _client._sequenceNumber++);
        BinaryPrimitives.WriteUInt32BigEndian(nonce[4..], _client._timestamp);
        _client._timestamp += Opus.FrameSamplesPerChannel;
        BinaryPrimitives.WriteUInt32BigEndian(nonce[8..], _client.Ssrc);

        nonce[..12].CopyTo(result);
        fixed (byte* resultPtr = result, bufferPtr = buffer)
            _ = Libsodium.CryptoSecretboxEasy(resultPtr + 12, bufferPtr, (ulong)bufferLen, noncePtr, _client._secretKey!);
    }
}
