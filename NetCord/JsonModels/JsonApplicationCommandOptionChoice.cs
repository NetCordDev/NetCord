using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonApplicationCommandOptionChoice
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("value")]
    public JsonElement Value { get; init; }
}