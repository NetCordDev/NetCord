using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling))]
public enum IntegrationType
{
    Twitch,
    YouTube,
    Discord
}
