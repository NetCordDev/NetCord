using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class VoiceStateOptions
{
    internal VoiceStateOptions()
    {
    }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; }

    [JsonPropertyName("suppress")]
    public bool? Suppress { get; set; }

    public VoiceStateOptions(Snowflake channelId)
    {
        ChannelId = channelId;
    }
}