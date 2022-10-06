using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class VoiceResumeProperties
{
    public VoiceResumeProperties(Snowflake guildId, string sessionId, string token)
    {
        GuildId = guildId;
        SessionId = sessionId;
        Token = token;
    }

    [JsonPropertyName("server_id")]
    public Snowflake GuildId { get; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; }

    [JsonPropertyName("token")]
    public string Token { get; }
}
