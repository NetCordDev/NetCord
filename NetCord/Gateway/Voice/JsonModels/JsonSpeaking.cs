using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

internal partial class JsonSpeaking
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; set; }

    [JsonPropertyName("speaking")]
    public SpeakingFlags Speaking { get; set; }
}
