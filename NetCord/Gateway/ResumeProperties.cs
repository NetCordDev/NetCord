using System.Text.Json.Serialization;

namespace NetCord.Gateway;

internal class ResumeProperties
{
    public ResumeProperties(string token, string sessionId, int sequenceNumber)
    {
        Token = token;
        SessionId = sessionId;
        SequenceNumber = sequenceNumber;
    }

    [JsonPropertyName("token")]
    public string Token { get; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; }

    [JsonPropertyName("seq")]
    public int SequenceNumber { get; }
}