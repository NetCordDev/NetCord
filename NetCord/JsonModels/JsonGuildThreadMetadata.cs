using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildThreadMetadata
{
    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    [JsonPropertyName("auto_archive_duration")]
    public int AutoArchiveDuration { get; set; }

    [JsonPropertyName("archive_timestamp")]
    public DateTimeOffset ArchiveTimestamp { get; set; }

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }

    [JsonPropertyName("invitable")]
    public bool? Invitable { get; set; }

    [JsonSerializable(typeof(JsonGuildThreadMetadata))]
    public partial class JsonGuildThreadMetadataSerializerContext : JsonSerializerContext
    {
        public static JsonGuildThreadMetadataSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
