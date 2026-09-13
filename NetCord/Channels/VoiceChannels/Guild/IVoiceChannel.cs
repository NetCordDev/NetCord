namespace NetCord;

public interface IVoiceChannel : IGuildMessageChannel
{
    /// <summary>
    /// The voice channel's bitrate (in bits per second).
    /// </summary>
    public int Bitrate { get; }

    /// <summary>
    /// The voice channel's user limit.
    /// </summary>
    public int UserLimit { get; }

    /// <inheritdoc cref="TextGuildChannel.Slowmode"/>
    public int Slowmode { get; }

    /// <inheritdoc cref="TextGuildChannel.ParentId"/>
    public ulong? ParentId { get; }

    /// <summary>
    /// The voice channel's preferred region, if set. When <see langword="null"/>, Discord automatically determines this.
    /// </summary>
    public string? RtcRegion { get; }

    /// <summary>
    /// The voice channel's video optimization mode (for cameras).
    /// </summary>
    public VideoQualityMode VideoQualityMode { get; }
}

public interface IVoiceGuildChannel :
    IVoiceChannel,
    INamedChannel,
    IPositionedGuildChannel,
    IPermissionOverwriteChannel,
    IInvitableGuildChannel
{
}

public interface IStageGuildChannel :
    IVoiceChannel,
    INamedChannel,
    IPositionedGuildChannel,
    IPermissionOverwriteChannel,
    IInvitableGuildChannel
{
}
