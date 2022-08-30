using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class VoiceStateOptions
{
    internal VoiceStateOptions(Snowflake channelId)
    {
        ChannelId = channelId;
    }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("suppress")]
    public bool? Suppress { get; set; }
}
