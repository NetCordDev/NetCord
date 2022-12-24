using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

public partial class JsonSpeaking
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; set; }

    [JsonPropertyName("speaking")]
    public SpeakingFlags Speaking { get; set; }

    [JsonSerializable(typeof(JsonSpeaking))]
    public partial class JsonSpeakingSerializerContext : JsonSerializerContext
    {
        public static JsonSpeakingSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
