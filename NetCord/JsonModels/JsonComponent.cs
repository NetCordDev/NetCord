using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonComponent
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

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

    [JsonPropertyName("sku_id")]
    public ulong? SkuId { get; set; }

    [JsonPropertyName("options")]
    public JsonMenuSelectOption[]? Options { get; set; }

    [JsonPropertyName("channel_types")]
    public ChannelType[]? ChannelTypes { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("default_values")]
    public JsonEntityMenuValue[]? DefaultValues { get; set; }

    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    [JsonPropertyName("components")]
    public JsonComponent[]? Components { get; set; }

    [JsonPropertyName("min_length")]
    public int? MinLength { get; set; }

    [JsonPropertyName("max_length")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("accessory")]
    public JsonComponent? Accessory { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("media")]
    public JsonComponentMedia? Media { get; set; }

    [JsonPropertyName("file")]
    public JsonComponentMedia? File { get; set; }

    [JsonPropertyName("spoiler")]
    public bool? Spoiler { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("divider")]
    public bool? Divider { get; set; }

    [JsonPropertyName("spacing")]
    public ComponentSeparatorSpacingSize? Spacing { get; set; }

    [JsonPropertyName("accent_color")]
    public Color? AccentColor { get; set; }

    [JsonPropertyName("items")]
    public JsonComponent[]? Items { get; set; }

    [JsonPropertyName("component")]
    public JsonComponent? Component { get; set; }

    [JsonPropertyName("values")]
    public string[]? SelectedValues { get; set; }
}
