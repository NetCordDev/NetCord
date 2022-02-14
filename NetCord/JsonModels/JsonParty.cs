using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonParty
{
    public string? Id { get; init; }

    [JsonPropertyName("size")]
    public int[]? Size { get; init; }
}