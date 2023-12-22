using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonVoiceRegion
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("optimal")]
    public bool Optimal { get; set; }

    [JsonPropertyName("deprecated")]
    public bool Deprecated { get; set; }

    [JsonPropertyName("custom")]
    public bool Custom { get; set; }
}
