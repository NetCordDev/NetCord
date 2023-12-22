using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonComponent
{
    [JsonPropertyName("type")]
    public ComponentType Type { get; set; }

    [JsonPropertyName("custom_id")]
    public string? CustomId { get; set; }

    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }

    [JsonPropertyName("style")]
    public ButtonStyle? Style { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("options")]
    public JsonMenuSelectOption[] Options { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    [JsonPropertyName("components")]
    public JsonComponent[] Components { get; set; }

    [JsonPropertyName("min_length")]
    public int? MinLength { get; set; }

    [JsonPropertyName("max_length")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
