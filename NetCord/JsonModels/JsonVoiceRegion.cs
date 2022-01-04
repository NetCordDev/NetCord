using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonVoiceRegion
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("optimal")]
    public bool Optimal { get; init; }

    [JsonPropertyName("deprecated")]
    public bool Deprecated { get; init; }

    [JsonPropertyName("custom")]
    public bool Custom { get; init; }
}