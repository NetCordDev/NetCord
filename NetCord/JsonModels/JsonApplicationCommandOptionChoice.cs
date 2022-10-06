using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonApplicationCommandOptionChoice
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    [JsonPropertyName("value")]
    public JsonElement Value { get; set; }

    [JsonSerializable(typeof(JsonApplicationCommandOptionChoice))]
    public partial class JsonApplicationCommandOptionChoiceSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandOptionChoiceSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
