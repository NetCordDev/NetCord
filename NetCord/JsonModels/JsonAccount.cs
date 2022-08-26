using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonAccount : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }
}
