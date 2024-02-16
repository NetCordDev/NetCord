using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class VoiceIdentifyProperties(ulong guildId, ulong userId, string sessionId, string token)
{
    [JsonPropertyName("server_id")]
    public ulong GuildId { get; set; } = guildId;

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; } = userId;

    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = sessionId;

    [JsonPropertyName("token")]
    public string Token { get; set; } = token;
}
