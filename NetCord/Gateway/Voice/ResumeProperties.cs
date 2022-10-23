using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class VoiceResumeProperties
{
    public VoiceResumeProperties(ulong guildId, string sessionId, string token)
    {
        GuildId = guildId;
        SessionId = sessionId;
        Token = token;
    }

    [JsonPropertyName("server_id")]
    public ulong GuildId { get; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; }

    [JsonPropertyName("token")]
    public string Token { get; }
}
