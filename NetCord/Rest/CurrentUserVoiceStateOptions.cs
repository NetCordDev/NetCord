using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class CurrentUserVoiceStateOptions
{
    internal CurrentUserVoiceStateOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("suppress")]
    public bool? Suppress { get; set; }

    [JsonConverter(typeof(JsonConverters.NullableDateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("request_to_speak_timestamp")]
    public DateTimeOffset? RequestToSpeakTimestamp { get; set; }
}
