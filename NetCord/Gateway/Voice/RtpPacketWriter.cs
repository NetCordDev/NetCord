using System.Buffers.Binary;

namespace NetCord.Gateway.Voice;

public readonly ref struct RtpPacketWriter(Span<byte> datagram, bool encryptedExtension)
{
    private readonly Span<byte> _datagram = datagram;

    public readonly ReadOnlySpan<byte> Datagram => _datagram;

    public readonly int Version => Datagram[0] >> 6;

    public readonly bool Padding => (Datagram[0] & 0b100000) is not 0;

    public readonly bool Extension => (Datagram[0] & 0b10000) is not 0;

    public readonly int CsrcCount => Datagram[0] & 0b1111;

    public readonly bool Marker => (Datagram[1] & 0b10000000) is not 0;

    public readonly int PayloadType => Datagram[1] & 0b1111111;

    public readonly ushort SequenceNumber => BinaryPrimitives.ReadUInt16BigEndian(Datagram[2..]);

    public readonly uint Timestamp => BinaryPrimitives.ReadUInt32BigEndian(Datagram[4..]);

    public readonly uint Ssrc => BinaryPrimitives.ReadUInt32BigEndian(Datagram[8..]);

    public readonly ReadOnlySpan<byte> FixedHeader => Datagram[..12];

    public readonly ReadOnlySpan<byte> Header => Datagram[..HeaderLength];

    public readonly ReadOnlySpan<byte> ExtendedHeader => Datagram[..ExtendedHeaderLength];

    public readonly int HeaderLength => 12 + (sizeof(int) * CsrcCount);

    public readonly int ExtendedHeaderLength => (encryptedExtension || !Extension ? 12 : 16) + (sizeof(int) * CsrcCount);

    public readonly Span<byte> Payload => _datagram[ExtendedHeaderLength..];
}
