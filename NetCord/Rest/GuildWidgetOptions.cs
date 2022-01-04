using System.Text.Json.Serialization;

namespace NetCord;

public class GuildWidgetOptions
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("channel_id")]
    public DiscordId? ChannelId { get; set; }
}