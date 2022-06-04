using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class SpeakingProperties
{
    public SpeakingProperties(SpeakingFlags speaking, int delay, uint ssrc)
    {
        Speaking = speaking;
        Delay = delay;
        Ssrc = ssrc;
    }

    [JsonPropertyName("speaking")]
    public SpeakingFlags Speaking { get; }

    [JsonPropertyName("delay")]
    public int Delay { get; }

    [JsonPropertyName("ssrc")]
    public uint Ssrc { get; }
}