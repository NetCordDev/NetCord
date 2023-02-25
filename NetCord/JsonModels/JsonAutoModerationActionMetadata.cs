using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonAutoModerationActionMetadata
{
    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("duration_seconds")]
    public int? DurationSeconds { get; set; }

    [JsonPropertyName("custom_message")]
    public string? CustomMessage { get; set; }

    [JsonSerializable(typeof(JsonAutoModerationActionMetadata))]
    public partial class JsonAutoModerationActionMetadataSerializerContext : JsonSerializerContext
    {
        public static JsonAutoModerationActionMetadataSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
