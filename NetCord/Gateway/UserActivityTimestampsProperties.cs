using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[GenerateMethodsForProperties]
public partial class UserActivityTimestampsProperties
{
    [JsonPropertyName("start")]
    public int? Start { get; set; }

    [JsonPropertyName("end")]
    public int? End { get; set; }
}
