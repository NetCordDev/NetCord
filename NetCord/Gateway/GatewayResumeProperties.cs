using System.Text.Json.Serialization;

namespace NetCord.Gateway;

internal class GatewayResumeProperties(string token, string sessionId, int sequenceNumber)
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = token;

    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = sessionId;

    [JsonPropertyName("seq")]
    public int SequenceNumber { get; set; } = sequenceNumber;
}
