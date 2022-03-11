using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling))]
public enum ConnectionType
{
    Twitch,
    Youtube,
}
