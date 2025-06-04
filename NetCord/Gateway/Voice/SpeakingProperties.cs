using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class SpeakingProperties(SpeakingFlags speaking, int delay)
{
    [JsonPropertyName("speaking")]
    public SpeakingFlags Speaking { get; set; } = speaking;

    [JsonPropertyName("delay")]
    public int Delay { get; set; } = delay;
}
