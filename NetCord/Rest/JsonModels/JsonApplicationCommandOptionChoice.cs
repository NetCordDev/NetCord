using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonApplicationCommandOptionChoice
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<string, string>? NameLocalizations { get; set; }

    [JsonPropertyName("value")]
    public JsonElement Value { get; set; }
}
