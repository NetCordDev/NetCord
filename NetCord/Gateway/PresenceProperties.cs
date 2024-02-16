using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class PresenceProperties(UserStatusType statusType)
{
    [JsonConverter(typeof(JsonConverters.MillisecondsNullableUnixDateTimeOffsetConverter))]
    [JsonPropertyName("since")]
    public DateTimeOffset? Since { get; set; }

    [JsonPropertyName("activities")]
    public IEnumerable<UserActivityProperties>? Activities { get; set; }

    [JsonPropertyName("status")]
    public UserStatusType StatusType { get; set; } = statusType;

    [JsonPropertyName("afk")]
    public bool Afk { get; set; }
}
