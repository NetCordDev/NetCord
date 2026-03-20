namespace NetCord.Gateway.Voice;

internal readonly struct RtpPacketStorage(ReadOnlyMemory<byte> datagram)
{
    public readonly RtpPacket Packet => new(datagram.Span);
}
