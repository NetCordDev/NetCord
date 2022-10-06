using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling<Platform>))]
public enum Platform
{
    Desktop,
    Mobile,
    Web,
}
