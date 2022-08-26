using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonApplicationCommandInteractionDataOption
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("type")]
    public ApplicationCommandOptionType Type { get; init; }

    [JsonPropertyName("value")]
    public JsonElement? Value { get; init; }

    [JsonPropertyName("options")]
    public JsonApplicationCommandInteractionDataOption[]? Options { get; init; }

    [JsonPropertyName("focused")]
    public bool Focused { get; init; }
}
