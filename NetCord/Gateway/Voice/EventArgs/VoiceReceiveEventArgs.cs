using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

[StructLayout(LayoutKind.Auto)]
public readonly ref struct VoiceReceiveEventArgs
{
    public VoiceReceiveEventArgs(ReadOnlySpan<byte> frame, uint ssrc, uint timestamp, ushort sequenceNumber)
    {
        Frame = frame;
        Ssrc = ssrc;
        Timestamp = timestamp;
        SequenceNumber = sequenceNumber;
    }

    /// <summary>
    /// The voice frame data.
    /// </summary>
    public readonly ReadOnlySpan<byte> Frame { get; }

    /// <summary>
    /// The synchronization source (SSRC) of the sender of the voice frame.
    /// </summary>
    public readonly uint Ssrc { get; }

    /// <summary>
    /// The timestamp of the voice frame.
    /// </summary>
    public readonly uint Timestamp { get; }

    /// <summary>
    /// The sequence number of the voice frame.
    /// </summary>
    public readonly ushort SequenceNumber { get; }
}
