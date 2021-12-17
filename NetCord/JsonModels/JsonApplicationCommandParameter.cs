using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonApplicationCommandParameter
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("type")]
    public ApplicationCommandParameterType Type { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandParameter[]? Parameters { get; init; }

    [JsonPropertyName("focused")]
    public bool? Focused { get; init; }
}
