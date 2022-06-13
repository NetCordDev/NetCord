using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildWidgetChannel : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("position")]
    public int Position { get; init; }
}