using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonMessageComponent
{
    [JsonPropertyName("type")]
    public MessageComponentType Type { get; init; }

    [JsonPropertyName("custom_id")]
    public string? CustomId { get; init; }

    [JsonPropertyName("disabled")]
    public bool Disabled { get; init; }

    [JsonPropertyName("style")]
    public MessageButtonStyle? Style { get; init; }

    [JsonPropertyName("label")]
    public string? Label { get; init; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("options")]
    public SelectOption[] Options { get; init; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; init; }

    [JsonPropertyName("min_values")]
    public int? MinValues { get; init; }

    [JsonPropertyName("max_values")]
    public int? MaxValues { get; init; }

    [JsonPropertyName("components")]
    public JsonMessageComponent[] Components { get; init; }

    internal record SelectOption
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
        public bool? IsDefault { get; init; }
    }
}
