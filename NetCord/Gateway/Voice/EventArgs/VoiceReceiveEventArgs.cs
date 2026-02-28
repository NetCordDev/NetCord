using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

[StructLayout(LayoutKind.Auto)]
public readonly ref struct VoiceReceiveEventArgs
{
    private VoiceReceiveEventArgs(ReadOnlySpan<byte> frame, uint fecAndSamples, uint ssrc, uint timestamp, ushort sequenceNumber)
    {
        _frame = frame;
        _fecAndSamples = fecAndSamples;
        Ssrc = ssrc;
        Timestamp = timestamp;
        SequenceNumber = sequenceNumber;
    }

    public static VoiceReceiveEventArgs Delivered(ReadOnlySpan<byte> frame, uint ssrc, uint timestamp, ushort sequenceNumber)
    {
        return new(frame, 0, ssrc, timestamp, sequenceNumber);
    }

    public static VoiceReceiveEventArgs Lost(uint ssrc, uint timestamp, ushort sequenceNumber, int samplesPerChannel, ReadOnlySpan<byte> fecData = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(samplesPerChannel);

        var fecAndSamples = (uint)samplesPerChannel | (fecData.IsEmpty ? 0 : ((uint)1 << 31));

        return new(fecData, fecAndSamples, ssrc, timestamp, sequenceNumber);
    }

    private readonly ReadOnlySpan<byte> _frame;

    private readonly uint _fecAndSamples;

    /// <summary>
    /// The voice frame data.
    /// </summary>
    public readonly ReadOnlySpan<byte> Frame => IsLost ? null : _frame;

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

    public readonly bool IsLost => _fecAndSamples is not 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LostVoiceReceiveEventArgs AsLost()
    {
        if (!IsLost)
            ThrowNotLost();

        return new(Ssrc, Timestamp, SequenceNumber, _frame, _fecAndSamples);
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
    internal LostVoiceReceiveEventArgs(uint ssrc, uint timestamp, ushort sequenceNumber, ReadOnlySpan<byte> fecData, uint fecAndSamples)
    {
        FecData = fecData;
        Ssrc = ssrc;
        Timestamp = timestamp;
        SequenceNumber = sequenceNumber;
        _fecAndSamples = fecAndSamples;
    }

    public readonly ReadOnlySpan<byte> FecData { get; }

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
    public readonly int SamplesPerChannel => (int)(_fecAndSamples & ~(1u << 31));

    /// <summary>
    /// Whether the frame should be decoded using FEC.
    /// </summary>
    public readonly bool DecodeFec => (_fecAndSamples & (1u << 31)) is not 0;

    private readonly uint _fecAndSamples;
}
