using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MessagePollProperties : MessagePollBase
{
    [JsonPropertyName("duration")]
    public int DurationInHours { get; set; }
}
