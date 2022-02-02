using System.Text.Json.Serialization;

namespace NetCord;

public class VoiceStateOptions
{
    [JsonPropertyName("channel_id")]
    public DiscordId ChannelId { get; }

    [JsonPropertyName("suppress")]
    public bool? Suppress { get; set; }

    public VoiceStateOptions(DiscordId channelId)
    {
        ChannelId = channelId;
    }
}