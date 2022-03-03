using System.Text.Json.Serialization;

namespace NetCord;

public class UserActivityTimestampsProperties
{
    [JsonPropertyName("start")]
    public int? Start { get; set; }

    [JsonPropertyName("end")]
    public int? End { get; set; }
}