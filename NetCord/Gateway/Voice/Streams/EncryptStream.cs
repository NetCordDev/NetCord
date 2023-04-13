using System.Buffers;
using System.Buffers.Binary;

using NetCord.Gateway.Voice.Encryption;

namespace NetCord.Gateway.Voice;

internal class EncryptStream : RewritingStream
{
    private readonly IVoiceEncryption _encryption;
    private readonly int _expansion;
    private readonly VoiceClient _client;
    private ushort _sequenceNumber;
    private uint _timestamp;

    public EncryptStream(Stream next, IVoiceEncryption encryption, VoiceClient client) : base(next)
    {
        _encryption = encryption;
        _expansion = encryption.Expansion + 12;
        _client = client;
        _sequenceNumber = (ushort)Random.Shared.Next(ushort.MaxValue);
        _timestamp = (uint)Random.Shared.Next(int.MinValue, int.MaxValue);
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        int datagramLen = buffer.Length + _expansion;
        using var owner = MemoryPool<byte>.Shared.Rent(datagramLen);
        var datagram = owner.Memory[..datagramLen];
        WriteDatagram(buffer.Span, datagram.Span);
        await _next.WriteAsync(datagram, cancellationToken).ConfigureAwait(false);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        int datagramLen = buffer.Length + _expansion;
        using var owner = MemoryPool<byte>.Shared.Rent(datagramLen);
        var datagram = owner.Memory.Span[..datagramLen];
        WriteDatagram(buffer, datagram);
        _next.Write(datagram);
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

    private void WriteDatagram(ReadOnlySpan<byte> buffer, Span<byte> datagram)
    {
        WriteRtpHeader(datagram);
        _encryption.Encrypt(buffer, datagram);
    }

    private void WriteRtpHeader(Span<byte> datagram)
    {
        datagram[0] = 0b10000000;
        datagram[1] = 0b1111000;
        BinaryPrimitives.WriteUInt16BigEndian(datagram[2..], ++_sequenceNumber);
        BinaryPrimitives.WriteUInt32BigEndian(datagram[4..], _timestamp += Opus.SamplesPerChannel);
        BinaryPrimitives.WriteUInt32BigEndian(datagram[8..], _client.Ssrc);
    }
}
