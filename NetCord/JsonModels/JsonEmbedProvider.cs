using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEmbedProvider
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
