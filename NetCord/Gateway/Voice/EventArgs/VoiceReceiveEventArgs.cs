namespace NetCord.Gateway.Voice;

public readonly ref struct VoiceReceiveEventArgs(byte[]? buffer, int frameIndex, int frameLength, uint ssrc, uint? timestamp, ushort sequenceNumber, bool canCorrectLoss)
{
    internal readonly byte[]? _buffer = buffer;

    /// <summary>
    /// The voice frame data.
    /// </summary>
    public ReadOnlySpan<byte> Frame => new(_buffer, frameIndex, frameLength);

    /// <summary>
    /// The synchronization source (SSRC) of the sender of the voice frame.
    /// </summary>
    public uint Ssrc => ssrc;

    /// <summary>
    /// The timestamp of the voice frame. <see langword="null"/> when the frame was lost.
    /// </summary>
    public uint? Timestamp => timestamp;

    /// <summary>
    /// The sequence number of the voice frame.
    /// </summary>
    public ushort SequenceNumber => sequenceNumber;

    public bool CanCorrectLoss => canCorrectLoss;
}
