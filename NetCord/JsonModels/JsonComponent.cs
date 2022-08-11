using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonComponent
{
    [JsonPropertyName("type")]
    public ComponentType Type { get; init; }

    [JsonPropertyName("custom_id")]
    public string? CustomId { get; init; }

    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }

    [JsonPropertyName("style")]
    public ButtonStyle? Style { get; init; }

    [JsonPropertyName("label")]
    public string? Label { get; init; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("options")]
    public JsonMenuSelectOption[] Options { get; init; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; init; }

    [JsonPropertyName("min_values")]
    public int? MinValues { get; init; }

    [JsonPropertyName("max_values")]
    public int? MaxValues { get; init; }

    [JsonPropertyName("components")]
    public JsonComponent[] Components { get; init; }

    [JsonPropertyName("min_length")]
    public int? MinLength { get; init; }

    [JsonPropertyName("max_length")]
    public int? MaxLength { get; init; }

    [JsonPropertyName("required")]
    public bool? Required { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }
}