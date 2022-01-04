using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonGuildScheduledEventMetadata
{
    [JsonPropertyName("location")]
    public string? Location { get; init; }
}