using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

internal record JsonSpeaking
{
    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; init; }

    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; init; }

    [JsonPropertyName("speaking")]
    public SpeakingFlags Speaking { get; init; }
}
