using System.Text.Json.Serialization;

namespace NetCord;

public class ApplicationCommandOptionChoiceProperties
{
    [JsonPropertyName("name")]
    public string Name { get; }

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