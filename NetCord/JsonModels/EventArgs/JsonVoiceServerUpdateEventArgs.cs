using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonVoiceServerUpdateEventArgs
{
    [JsonPropertyName("token")]
    public string Token { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; init; }
}