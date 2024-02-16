using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class TextInputProperties(string customId, TextInputStyle style, string label)
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.TextInput;

    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; } = customId;

    [JsonPropertyName("style")]
    public TextInputStyle Style { get; set; } = style;

    [JsonPropertyName("label")]
    public string Label { get; set; } = label;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("min_length")]
    public int? MinLength { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("max_length")]
    public int? MaxLength { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }
}
