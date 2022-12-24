using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonApplicationCommandOption
{
    [JsonPropertyName("type")]
    public ApplicationCommandOptionType Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("choices")]
    public JsonApplicationCommandOptionChoice[]? Choices { get; set; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandOption[]? Options { get; set; }

    [JsonPropertyName("channel_types")]
    public ChannelType[]? ChannelTypes { get; set; }

    [JsonPropertyName("min_value")]
    public double? MinValue { get; set; }

    [JsonPropertyName("max_value")]
    public double? MaxValue { get; set; }

    [JsonPropertyName("min_length")]
    public int? MinLength { get; set; }

    [JsonPropertyName("max_length")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("autocomplete")]
    public bool Autocomplete { get; set; }

    [JsonSerializable(typeof(JsonApplicationCommandOption))]
    public partial class JsonApplicationCommandOptionSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandOptionSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
