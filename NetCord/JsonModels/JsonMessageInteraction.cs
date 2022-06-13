using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonMessageInteraction : JsonEntity
{
    [JsonPropertyName("type")]
    public InteractionType Type { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("user")]
    public JsonUser User { get; init; }
}
