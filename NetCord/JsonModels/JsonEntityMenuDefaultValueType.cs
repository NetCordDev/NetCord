using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.JsonModels;

[JsonConverter(typeof(SafeStringEnumConverter<JsonEntityMenuDefaultValueType>))]
public enum JsonEntityMenuDefaultValueType : sbyte
{
    [JsonPropertyName("user")]
    User,

    [JsonPropertyName("role")]
    Role,

    [JsonPropertyName("channel")]
    Channel,
}
