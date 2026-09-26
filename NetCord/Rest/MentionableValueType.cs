using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.Rest;

[JsonConverter(typeof(SafeStringEnumConverter<MentionableValueType>))]
public enum MentionableValueType : sbyte
{
    [JsonPropertyName("user")]
    User,

    [JsonPropertyName("role")]
    Role,
}
