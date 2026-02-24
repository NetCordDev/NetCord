using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

[StructLayout(LayoutKind.Auto)]
public readonly ref struct VoiceReceiveData
{
    private VoiceReceiveData(uint ssrc, ushort sequenceNumber, RtpPacket packet, bool hasPacket)
    {
        Ssrc = ssrc;
        SequenceNumber = sequenceNumber;
        Packet = packet;
        HasPacket = hasPacket;
    }

    public static VoiceReceiveData FromPacket(RtpPacket packet)
    {
        return new(packet.Ssrc, packet.SequenceNumber, packet, true);
    }

    public static VoiceReceiveData Missed(uint ssrc, ushort sequenceNumber)
    {
        return new(ssrc, sequenceNumber, default, false);
    }

    public RtpPacket Packet { get; }

    public uint Ssrc { get; }

    public ushort SequenceNumber { get; }

    public bool HasPacket { get; }
}
