namespace NetCord.Gateway.Voice;

public readonly ref struct VoiceReceiveEventArgs(byte[]? buffer, int frameIndex, int frameLength, uint ssrc, uint? timestamp, ushort sequenceNumber)
{
    internal readonly byte[]? _buffer = buffer;

    public ReadOnlySpan<byte> Frame => new(_buffer, frameIndex, frameLength);

    public uint Ssrc => ssrc;

    public uint? Timestamp => timestamp;

    public ushort SequenceNumber => sequenceNumber;
}
