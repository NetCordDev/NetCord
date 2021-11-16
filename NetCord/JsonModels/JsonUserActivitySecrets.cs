using System.Text.Json.Serialization;

namespace NetCord.JsonModels;
internal record JsonUserActivitySecrets
{
    [JsonPropertyName("join")]
    public string? Join { get; init; }

    [JsonPropertyName("spectate")]
    public string? Spectate { get; init; }

    [JsonPropertyName("match")]
    public string? Match { get; init; }
}
