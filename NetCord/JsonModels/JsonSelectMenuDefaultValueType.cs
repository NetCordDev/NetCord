using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.JsonModels;

[JsonConverter(typeof(SafeStringEnumConverter<JsonSelectMenuDefaultValueType>))]
public enum JsonSelectMenuDefaultValueType : sbyte
{
    [JsonPropertyName("user")]
    User,

    [JsonPropertyName("role")]
    Role,

    [JsonPropertyName("channel")]
    Channel,
}
