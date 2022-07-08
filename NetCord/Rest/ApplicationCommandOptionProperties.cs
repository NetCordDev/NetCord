using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class ApplicationCommandOptionProperties
{
    [JsonPropertyName("type")]
    public ApplicationCommandOptionType Type { get; }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("required")]
    public bool? Required { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("choices")]
    public IEnumerable<ApplicationCommandOptionChoiceProperties>? Choices { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("options")]
    public IEnumerable<ApplicationCommandOptionProperties>? Options { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_types")]
    public IEnumerable<ChannelType>? ChannelTypes { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("min_value")]
    public double? MinValue { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("max_value")]
    public double? MaxValue { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("min_length")]
    public int? MinLength { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("max_length")]
    public int? MaxLength { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("autocomplete")]
    public bool? Autocomplete { get; set; }

    public ApplicationCommandOptionProperties(ApplicationCommandOptionType type, string name, string description)
    {
        Type = type;
        Name = name;
        Description = description;
    }
}