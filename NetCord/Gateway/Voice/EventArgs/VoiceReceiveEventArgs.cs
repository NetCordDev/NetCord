namespace NetCord.Gateway.Voice;

public readonly ref struct VoiceReceiveEventArgs(byte[]? buffer, int frameIndex, int frameLength, uint ssrc)
{
    internal readonly byte[]? _buffer = buffer;

    public ReadOnlySpan<byte> Frame => new(_buffer, frameIndex, frameLength);

    public uint Ssrc => ssrc;
}
