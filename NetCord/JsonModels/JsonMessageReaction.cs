using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonMessageReaction
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("me")]
    public bool Me { get; init; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; init; }
}