using System.Text.Json.Serialization;

namespace NetCord;

public class CurrentUserVoiceStateOptions : VoiceStateOptions
{
    [JsonPropertyName("request_to_speak_timestamp")]
    public DateTimeOffset? RequestToSpeakTimestamp { get; set; }

    public CurrentUserVoiceStateOptions(DiscordId channelId) : base(channelId)
    {
    }
}