using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.JsonModels;

[JsonConverter(typeof(SafeStringEnumConverter<JsonEntitySelectDefaultValueType>))]
public enum JsonEntitySelectDefaultValueType : sbyte
{
    [JsonPropertyName("user")]
    User,

    [JsonPropertyName("role")]
    Role,

    [JsonPropertyName("channel")]
    Channel,
}
