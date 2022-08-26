using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class ApplicationCommandOptionChoiceProperties
{
    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    [JsonPropertyName("value")]
    public object Value { get; }

    public ApplicationCommandOptionChoiceProperties(string name, string stringValue)
    {
        Name = name;
        Value = stringValue;
    }

    public ApplicationCommandOptionChoiceProperties(string name, double numericValue)
    {
        Name = name;
        Value = numericValue;
    }
}
