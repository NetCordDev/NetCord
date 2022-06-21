using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GuildWidgetSettingsOptions
{
    internal GuildWidgetSettingsOptions()
    {
    }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; set; }
}