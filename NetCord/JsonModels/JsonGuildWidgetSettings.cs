using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildWidgetSettings
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; init; }
}