using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.JsonModels;

[JsonConverter(typeof(SafeStringEnumConverter<JsonEntityMenuValueType>))]
public enum JsonEntityMenuValueType : sbyte
{
    [JsonPropertyName("user")]
    User,

    [JsonPropertyName("role")]
    Role,

    [JsonPropertyName("channel")]
    Channel,
}
