using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BufferedVoiceReceiveHandlerTest;

public readonly ref struct RtpPacket
{
    public RtpPacket(uint ssrc, uint timestamp, int payloadType, ushort sequenceNumber, int tag = 0)
    {
        Ssrc = ssrc;
        Timestamp = timestamp;
        PayloadType = payloadType;
        SequenceNumber = sequenceNumber;
        Tag = tag;
    }

    public unsafe RtpPacket(ReadOnlySpan<byte> datagram)
    {
        if (datagram.Length < sizeof(RtpPacket))
            throw new ArgumentException("Datagram is too short to be a valid RTP packet.", nameof(datagram));

        this = Unsafe.ReadUnaligned<RtpPacket>(ref MemoryMarshal.GetReference(datagram));
    }

    public unsafe ReadOnlySpan<byte> Datagram
    {
        get
        {
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<RtpPacket, byte>(ref Unsafe.AsRef(in this)), sizeof(RtpPacket));
        }
    }

    public uint Ssrc { get; }

    public uint Timestamp { get; }

    public int PayloadType { get; }

    public ushort SequenceNumber { get; }

    public int Tag { get; }
}
