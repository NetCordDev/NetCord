using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class PresenceProperties
{
    public PresenceProperties(UserStatusType statusType)
    {
        StatusType = statusType;
    }

    [JsonConverter(typeof(JsonConverters.MillisecondsNullableUnixDateTimeOffsetConverter))]
    [JsonPropertyName("since")]
    public DateTimeOffset? Since { get; set; }

    [JsonPropertyName("activities")]
    public IEnumerable<UserActivityProperties>? Activities { get; set; }

    [JsonPropertyName("status")]
    public UserStatusType StatusType { get; set; }

    [JsonPropertyName("afk")]
    public bool Afk { get; set; }
}
