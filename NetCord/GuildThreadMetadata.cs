using NetCord.JsonModels;

namespace NetCord;

public class GuildThreadMetadata(JsonGuildThreadMetadata jsonModel)
{
    public bool Archived { get; } = jsonModel.Archived;

    public ThreadArchiveDuration AutoArchiveDuration { get; } = jsonModel.AutoArchiveDuration;

    public DateTimeOffset ArchiveTimestamp { get; } = jsonModel.ArchiveTimestamp;

    public bool Locked { get; } = jsonModel.Locked;

    public bool? Invitable { get; } = jsonModel.Invitable;

    public DateTimeOffset? CreateTimestamp { get; } = jsonModel.CreateTimestamp;
}

