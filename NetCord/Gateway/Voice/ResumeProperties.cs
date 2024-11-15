using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class VoiceResumeProperties(ulong guildId, string sessionId, string token, int sequenceNumber)
{
    [JsonPropertyName("server_id")]
    public ulong GuildId { get; set; } = guildId;

    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = sessionId;

    [JsonPropertyName("token")]
    public string Token { get; set; } = token;

    [JsonPropertyName("seq_ack")]
    public int SequenceNumber { get; set; } = sequenceNumber;
}
