using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonApplicationCommandOption
{
    [JsonPropertyName("type")]
    public ApplicationCommandOptionType Type { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("required")]
    public bool Required { get; init; }

    [JsonPropertyName("choices")]
    public JsonApplicationCommandOptionChoice[]? Choices { get; init; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandOption[]? Options { get; init; }

    [JsonPropertyName("channel_types")]
    public ChannelType[]? ChannelTypes { get; init; }

    [JsonPropertyName("min_value")]
    public double? MinValue { get; init; }

    [JsonPropertyName("max_value")]
    public double? MaxValue { get; init; }

    [JsonPropertyName("autocomplete")]
    public bool Autocomplete { get; init; }
}