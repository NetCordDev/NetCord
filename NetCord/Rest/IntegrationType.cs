using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling<IntegrationType>))]
public enum IntegrationType
{
    Twitch,
    YouTube,
    Discord
}
