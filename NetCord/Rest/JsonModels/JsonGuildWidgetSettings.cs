using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonGuildWidgetSettings
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }
}
