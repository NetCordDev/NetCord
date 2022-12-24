using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonMessageReaction
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("me")]
    public bool Me { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; set; }

    [JsonSerializable(typeof(JsonMessageReaction))]
    public partial class JsonMessageReactionSerializerContext : JsonSerializerContext
    {
        public static JsonMessageReactionSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
