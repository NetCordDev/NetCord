using System.Buffers;
using System.Buffers.Binary;

namespace NetCord.Gateway.Voice;

internal class SodiumEncryptStream : RewritingStream
{
    private readonly VoiceClient _client;
    private ushort _sequenceNumber;
    private uint _timestamp;

    public SodiumEncryptStream(Stream next, VoiceClient client) : base(next)
    {
        _client = client;
        _sequenceNumber = (ushort)Random.Shared.Next(ushort.MaxValue);
        _timestamp = (uint)Random.Shared.Next(int.MinValue, int.MaxValue);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var bufferLen = buffer.Length;
        int resultLen = bufferLen + Libsodium.MacBytes + 12;
        using var owner = MemoryPool<byte>.Shared.Rent(resultLen);
        var result = owner.Memory[..resultLen];
        Encrypt(result.Span, buffer.Span, bufferLen);
        await _next.WriteAsync(result, cancellationToken).ConfigureAwait(false);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        int bufferLen = buffer.Length;
        int resultLen = bufferLen + Libsodium.MacBytes + 12;
        using var owner = MemoryPool<byte>.Shared.Rent(resultLen);
        var result = owner.Memory.Span[..resultLen];
        Encrypt(result, buffer, bufferLen);
        _next.Write(result);
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        ReadOnlyMemory<byte> bytes = new byte[] { 0xF8, 0xFF, 0xFE };
        for (int i = 0; i < 5; i++)
            await WriteAsync(bytes, cancellationToken).ConfigureAwait(false);

        await base.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public override void Flush()
    {
        ReadOnlySpan<byte> bytes = stackalloc byte[] { 0xF8, 0xFF, 0xFE };
        for (int i = 0; i < 5; i++)
            Write(bytes);

        base.Flush();
    }

    private unsafe void Encrypt(Span<byte> result, ReadOnlySpan<byte> buffer, int bufferLen)
    {
        var noncePtr = stackalloc byte[Libsodium.NonceBytes];
        Span<byte> nonce = new(noncePtr, Libsodium.NonceBytes);
        nonce[0] = 0x80;
        nonce[1] = 0x78;
        BinaryPrimitives.WriteUInt16BigEndian(nonce[2..], ++_sequenceNumber);
        BinaryPrimitives.WriteUInt32BigEndian(nonce[4..], _timestamp += Opus.SamplesPerChannel);
        BinaryPrimitives.WriteUInt32BigEndian(nonce[8..], _client.Ssrc);

        nonce[..12].CopyTo(result);
        int code;
        fixed (byte* resultPtr = result, bufferPtr = buffer)
            code = Libsodium.CryptoSecretboxEasy(resultPtr + 12, bufferPtr, (ulong)bufferLen, noncePtr, _client._secretKey!);

        if (code != 0)
            throw new LibsodiumException();
    }
}
