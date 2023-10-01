using System.Text.Json.Serialization;

namespace NetCord.Rest;

public enum MentionableValueType : byte
{
    [JsonPropertyName("user")]
    User,

    [JsonPropertyName("role")]
    Role,
}
