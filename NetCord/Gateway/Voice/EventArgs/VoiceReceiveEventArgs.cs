using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

[StructLayout(LayoutKind.Auto)]
public readonly ref struct VoiceReceiveEventArgs
{
    internal const uint FecFlag = 1u << 31;

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
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(samplesPerChannel);

        var fecAndSamples = (uint)samplesPerChannel | (fecData.IsEmpty ? 0 : FecFlag);

        return new(fecData, fecAndSamples, ssrc, timestamp, sequenceNumber);
    }

    private readonly ReadOnlySpan<byte> _frame;

    private readonly uint _fecAndSamples;

    /// <summary>
    /// The voice frame data. Empty if the frame was lost.
    /// </summary>
    public readonly ReadOnlySpan<byte> Frame
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IsLost ? null : _frame;
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
    /// Whether the voice frame was lost. If true, the <see cref="Frame"/> property will be empty 
    /// and the <see cref="AsLost"/> method can be used to retrieve the lost-specific data.
    /// </summary>
    public readonly bool IsLost
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _fecAndSamples is not 0;
    }

    /// <summary>
    /// Converts this <see cref="VoiceReceiveEventArgs"/> to a <see cref="LostVoiceReceiveEventArgs"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the voice frame was successfully delivered and is not considered lost.</exception>
    /// <returns><see cref="LostVoiceReceiveEventArgs"/> containing the lost-specific data of this event args.</returns>
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

[StructLayout(LayoutKind.Auto)]
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

    /// <summary>
    /// The FEC data of the lost voice frame, if any. This will be empty if no FEC data is available for the lost frame.
    /// </summary>
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
    public readonly int SamplesPerChannel
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)(_fecAndSamples & ~VoiceReceiveEventArgs.FecFlag);
    }

    /// <summary>
    /// Whether the frame should be decoded using FEC.
    /// </summary>
    public readonly bool DecodeFec
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_fecAndSamples & VoiceReceiveEventArgs.FecFlag) is not 0;
    }

    private readonly uint _fecAndSamples;
}
