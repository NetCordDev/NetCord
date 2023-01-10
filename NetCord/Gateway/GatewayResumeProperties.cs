using System.Text.Json.Serialization;

namespace NetCord.Gateway;

internal class GatewayResumeProperties
{
    public GatewayResumeProperties(string token, string sessionId, int sequenceNumber)
    {
        Token = token;
        SessionId = sessionId;
        SequenceNumber = sequenceNumber;
    }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; set; }

    [JsonPropertyName("seq")]
    public int SequenceNumber { get; set; }
}
