using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonParty
{
    public string? Id { get; init; }

    [JsonPropertyName("size")]
    public int[]? Size { get; init; }
}
