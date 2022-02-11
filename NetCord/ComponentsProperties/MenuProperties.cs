using System.Text.Json.Serialization;

namespace NetCord;

public class MenuProperties : ComponentProperties
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; }

    [JsonPropertyName("options")]
    public IEnumerable<MenuSelectOptionProperties>? Options { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    public MenuProperties(string customId) : base(ComponentType.Menu)
    {
        CustomId = customId;
    }
}

public class MenuSelectOptionProperties
{
    [JsonPropertyName("label")]
    public string Label { get; }

    [JsonPropertyName("value")]
    public string Value { get; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("emoji")]
    [JsonConverter(typeof(JsonConverters.ComponentEmojiConverter))]
    public DiscordId? EmojiId { get; set; }

    [JsonPropertyName("default")]
    public bool? IsDefault { get; set; }

    public MenuSelectOptionProperties(string label, string value)
    {
        Label = label;
        Value = value;
    }
}