using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonMessageReaction
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("me")]
    public bool Me { get; init; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; init; }
}
