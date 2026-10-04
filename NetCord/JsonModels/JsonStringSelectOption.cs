using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonStringSelectOption
{
    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("value")]
    public required string Value { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }

    [JsonPropertyName("default")]
    public bool? Default { get; set; }
}
