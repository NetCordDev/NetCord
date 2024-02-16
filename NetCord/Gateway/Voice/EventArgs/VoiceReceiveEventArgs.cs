namespace NetCord.Gateway.Voice;

public class VoiceReceiveEventArgs(uint ssrc, ulong userId, ReadOnlyMemory<byte> frame)
{
    public uint Ssrc { get; } = ssrc;

    public ulong UserId { get; } = userId;

    /// <summary>
    /// Opus frame.
    /// </summary>
    public ReadOnlyMemory<byte> Frame { get; } = frame;
}
