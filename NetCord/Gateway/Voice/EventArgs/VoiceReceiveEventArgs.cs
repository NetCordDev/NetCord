namespace NetCord.Gateway.Voice;

public class VoiceReceiveEventArgs(uint ssrc, ulong userId, ReadOnlyMemory<byte> frame)
{
    /// <summary>
    /// The SSRC of the user.
    /// </summary>
    public uint Ssrc => ssrc;

    /// <summary>
    /// The ID of the user.
    /// </summary>
    public ulong UserId => userId;

    /// <summary>
    /// The frame encoded in Opus.
    /// </summary>
    public ReadOnlyMemory<byte> Frame => frame;
}
