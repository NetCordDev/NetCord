using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class VoiceHeartbeatProperties(int timestamp, int sequenceNumber)
{
    [JsonPropertyName("t")]
    public int Timestamp { get; set; } = timestamp;

    [JsonPropertyName("seq_ack")]
    public int SequenceNumber { get; set; } = sequenceNumber;
}
