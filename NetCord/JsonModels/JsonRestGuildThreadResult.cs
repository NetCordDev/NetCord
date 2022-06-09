using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonRestGuildThreadResult
{
    [JsonPropertyName("threads")]
    public JsonChannel[] Threads { get; init; }

    [JsonPropertyName("members")]
    public JsonThreadUser[] Users { get; init; }
}

internal record JsonRestGuildThreadPartialResult : JsonRestGuildThreadResult
{
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }
}