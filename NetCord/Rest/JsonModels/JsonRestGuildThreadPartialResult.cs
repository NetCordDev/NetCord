using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonRestGuildThreadPartialResult : JsonRestGuildThreadResult
{
    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}
