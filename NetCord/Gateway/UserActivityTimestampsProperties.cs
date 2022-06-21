using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public class UserActivityTimestampsProperties
{
    [JsonPropertyName("start")]
    public int? Start { get; set; }

    [JsonPropertyName("end")]
    public int? End { get; set; }
}