using System.Text.Json.Serialization;

namespace NetCord;

public class GuildWidgetSettingsOptions
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("channel_id")]
    public DiscordId? ChannelId { get; set; }
}