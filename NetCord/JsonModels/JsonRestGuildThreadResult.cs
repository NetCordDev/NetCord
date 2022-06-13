using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonRestGuildThreadResult
{
    [JsonPropertyName("threads")]
    public JsonChannel[] Threads { get; init; }

    [JsonPropertyName("members")]
    public JsonThreadUser[] Users { get; init; }
}

public record JsonRestGuildThreadPartialResult : JsonRestGuildThreadResult
{
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }
}