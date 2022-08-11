using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class MenuSelectOptionProperties
{
    [JsonPropertyName("label")]
    public string Label { get; }

    [JsonPropertyName("value")]
    public string Value { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("default")]
    public bool Default { get; set; }

    public MenuSelectOptionProperties(string label, string value)
    {
        Label = label;
        Value = value;
    }
}