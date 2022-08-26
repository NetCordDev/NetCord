using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling))]
public enum ConnectionType
{
    Twitch,
    Youtube,
}
