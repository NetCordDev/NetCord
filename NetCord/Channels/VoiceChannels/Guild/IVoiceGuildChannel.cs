namespace NetCord;

public interface IVoiceGuildChannel : IGuildChannel
{
    public bool Nsfw { get; }
    public int Bitrate { get; }
    public int UserLimit { get; }
    public int Slowmode { get; }
    public ulong? ParentId { get; }
    public string? RtcRegion { get; }
    public VideoQualityMode VideoQualityMode { get; }
}
