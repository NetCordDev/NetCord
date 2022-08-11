using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonMenuSelectOption
{
    [JsonPropertyName("label")]
    public string Label { get; init; }

    [JsonPropertyName("value")]
    public string Value { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; init; }

    [JsonPropertyName("default")]
    public bool? Default { get; init; }
}