using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling<ConnectionType>))]
public enum ConnectionType
{
    Twitch,
    Youtube,
}
