using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonThreadMetadata
{
    [JsonPropertyName("archived")]
    public bool Archived { get; init; }

    [JsonPropertyName("auto_archive_duration")]
    public int AutoArchiveDuration { get; init; }

    [JsonPropertyName("archive_timestamp")]
    public DateTimeOffset ArchiveTimestamp { get; init; }

    [JsonPropertyName("locked")]
    public bool Locked { get; init; }

    [JsonPropertyName("invitable")]
    public bool? Invitable { get; init; }
}
