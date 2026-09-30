using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonVoiceRegion
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("optimal")]
    public required bool Optimal { get; set; }

    [JsonPropertyName("deprecated")]
    public required bool Deprecated { get; set; }

    [JsonPropertyName("custom")]
    public required bool Custom { get; set; }
}
