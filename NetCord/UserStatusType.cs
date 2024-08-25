using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.SafeStringEnumConverter<UserStatusType>))]
public enum UserStatusType
{
    [JsonPropertyName("online")]
    Online,

    [JsonPropertyName("dnd")]
    DoNotDisturb,

    [JsonPropertyName("idle")]
    Idle,

    [JsonPropertyName("invisible")]
    Invisible,

    [JsonPropertyName("offline")]
    Offline,
}
