using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

public readonly ref struct VoiceReceiveEventArgs
{
    private VoiceReceiveEventArgs(ReadOnlySpan<byte> frame, uint ssrc, uint timestamp, int samplesPerChannel, ushort sequenceNumber)
    {
        Frame = frame;
        Ssrc = ssrc;
        Timestamp = timestamp;
        _samplesPerChannel = samplesPerChannel;
        SequenceNumber = sequenceNumber;
    }

    public static VoiceReceiveEventArgs Delievered(ReadOnlySpan<byte> frame, uint ssrc, uint timestamp, ushort sequenceNumber)
    {
        return new(frame, ssrc, timestamp, -1, sequenceNumber);
    }

    public static VoiceReceiveEventArgs Lost(uint ssrc, uint timestamp, ushort sequenceNumber, int samplesPerChannel)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(samplesPerChannel);

        return new(null, ssrc, timestamp, samplesPerChannel, sequenceNumber);
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

    private readonly int _samplesPerChannel;

    /// <summary>
    /// The sequence number of the voice frame.
    /// </summary>
    public readonly ushort SequenceNumber { get; }

    public readonly bool IsLost => _samplesPerChannel < 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LostVoiceReceiveEventArgs AsLost()
    {
        if (!IsLost)
            ThrowNotLost();

        return new(Ssrc, Timestamp, SequenceNumber, _samplesPerChannel);
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowNotLost()
    {
        throw new InvalidOperationException("This event args is not lost.");
    }
}

public readonly ref struct LostVoiceReceiveEventArgs
{
    internal LostVoiceReceiveEventArgs(uint ssrc, uint timestamp, ushort sequenceNumber, int samplesPerChannel)
    {
        Ssrc = ssrc;
        Timestamp = timestamp;
        SequenceNumber = sequenceNumber;
        SamplesPerChannel = samplesPerChannel;
    }

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

    /// <summary>
    /// The number of samples per channel in the lost voice frame.
    /// </summary>
    public readonly int SamplesPerChannel { get; }
}
