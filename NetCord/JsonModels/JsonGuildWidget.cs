using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonGuildWidget
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; }

    [JsonPropertyName("channel_id")]
    public DiscordId? ChannelId { get; init; }
}