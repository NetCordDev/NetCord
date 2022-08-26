using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildScheduledEventMetadata
{
    [JsonPropertyName("location")]
    public string? Location { get; init; }
}
