using System.Text.Json.Serialization;

namespace NetCord;

public class CurrentUserVoiceStateOptions : VoiceStateOptions
{
    internal CurrentUserVoiceStateOptions(Snowflake channelId) : base(channelId)
    {
    }

    [JsonPropertyName("request_to_speak_timestamp")]
    public DateTimeOffset? RequestToSpeakTimestamp { get; set; }
}