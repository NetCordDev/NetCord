using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMenuSelectOption
{
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }

    [JsonPropertyName("default")]
    public bool Default { get; set; }
}
