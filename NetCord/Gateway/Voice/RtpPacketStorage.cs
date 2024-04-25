namespace NetCord.Gateway.Voice;

internal readonly struct RtpPacketStorage(ReadOnlyMemory<byte> datagram, bool encryptedExtension)
{
    public readonly RtpPacket Packet => new(datagram.Span, encryptedExtension);
}
