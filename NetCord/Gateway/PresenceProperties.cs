using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public class PresenceProperties
{
    public PresenceProperties(UserStatusType statusType, bool afk)
    {
        StatusType = statusType;
        Afk = afk;
    }

    [JsonPropertyName("since")]
    public int? Since { get; set; }

    [JsonPropertyName("activities")]
    public IEnumerable<UserActivityProperties>? Activities { get; set; }

    [JsonPropertyName("status")]
    public UserStatusType StatusType { get; }

    [JsonPropertyName("afk")]
    public bool Afk { get; }
}
