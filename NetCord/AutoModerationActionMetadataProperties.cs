using System.Text.Json.Serialization;

namespace NetCord;

public partial class AutoModerationActionMetadataProperties
{
    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("duration_seconds")]
    public int? DurationSeconds { get; set; }

    [JsonSerializable(typeof(AutoModerationActionMetadataProperties))]
    public partial class AutoModerationActionMetadataPropertiesSerializerContext : JsonSerializerContext
    {
        public static AutoModerationActionMetadataPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
