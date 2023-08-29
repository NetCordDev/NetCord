using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.StringEnumConverterWithErrorHandling<TeamRole>))]
public enum TeamRole
{
    [JsonPropertyName("admin")]
    Admin,

    [JsonPropertyName("developer")]
    Developer,

    [JsonPropertyName("read-only")]
    ReadOnly,
}
