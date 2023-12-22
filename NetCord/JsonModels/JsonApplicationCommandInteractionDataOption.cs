using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonApplicationCommandInteractionDataOption
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public ApplicationCommandOptionType Type { get; set; }

    [JsonConverter(typeof(JsonConverters.AnyValueToStringConverter))]
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandInteractionDataOption[]? Options { get; set; }

    [JsonPropertyName("focused")]
    public bool Focused { get; set; }
}
