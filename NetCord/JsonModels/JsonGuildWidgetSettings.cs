using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonGuildWidgetSettings
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; init; }
}