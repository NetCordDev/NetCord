using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class StringMenuSelectOptionProperties(string label, string value)
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = label;

    [JsonPropertyName("value")]
    public string Value { get; set; } = value;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("default")]
    public bool Default { get; set; }
}
