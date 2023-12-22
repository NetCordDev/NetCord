using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEmbedField
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("inline")]
    public bool Inline { get; set; }
}
