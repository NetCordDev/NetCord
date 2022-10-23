using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class VoiceIdentifyProperties
{
    public VoiceIdentifyProperties(ulong guildId, ulong userId, string sessionId, string token)
    {
        GuildId = guildId;
        UserId = userId;
        SessionId = sessionId;
        Token = token;
    }

    [JsonPropertyName("server_id")]
    public ulong GuildId { get; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; }

    [JsonPropertyName("session_id")]
    public string SessionId { get; }

    [JsonPropertyName("token")]
    public string Token { get; }
}
