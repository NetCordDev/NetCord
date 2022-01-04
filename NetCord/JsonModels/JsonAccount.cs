using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonAccount : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }
}