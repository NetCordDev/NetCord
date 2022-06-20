using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonAutoModerationActionMetadata
{
    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; init; }

    [JsonPropertyName("duration_seconds")]
    public int? DurationSeconds { get; init; }
}