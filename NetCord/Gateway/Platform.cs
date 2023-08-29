using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling<Platform>))]
public enum Platform
{
    [JsonPropertyName("desktop")]
    Desktop,

    [JsonPropertyName("mobile")]
    Mobile,

    [JsonPropertyName("web")]
    Web,
}
