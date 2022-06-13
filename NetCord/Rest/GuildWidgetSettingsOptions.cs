using System.Text.Json.Serialization;

namespace NetCord;

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