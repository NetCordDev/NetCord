using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

public partial class SpeakingProperties(SpeakingFlags speaking)
{
    [JsonPropertyName("speaking")]
    public SpeakingFlags Speaking { get; set; } = speaking;

    [JsonPropertyName("delay")]
    public int Delay { get; set; }
}
