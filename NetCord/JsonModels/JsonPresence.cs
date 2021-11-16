using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonPresence
{
    [JsonPropertyName("user")]
    public JsonUser User { get; private init; }

    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; private init; }

    [JsonPropertyName("status")]
    public UserStatus Status { get; private init; }

    [JsonPropertyName("activities")]
    public JsonUserActivity[] Activities { get; private init; }

    [JsonPropertyName("client_status")]
    public Dictionary<Platform, UserStatus> Platform { get; private init; }
}
