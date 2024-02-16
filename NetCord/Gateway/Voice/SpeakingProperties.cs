using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class SpeakingProperties(SpeakingFlags speaking, int delay, uint ssrc)
{
    [JsonPropertyName("speaking")]
    public SpeakingFlags Speaking { get; set; } = speaking;

    [JsonPropertyName("delay")]
    public int Delay { get; set; } = delay;

    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; set; } = ssrc;
}
