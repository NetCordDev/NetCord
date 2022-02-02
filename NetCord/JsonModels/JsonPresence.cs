using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonPresence
{
    [JsonPropertyName("user")]
    public JsonEntity User { get; init; }

    [JsonPropertyName("guild_id")]
    public DiscordId GuildId { get; init; }

    [JsonPropertyName("status")]
    public UserStatus Status { get; init; }

    [JsonPropertyName("activities")]
    public JsonUserActivity[] Activities { get; init; }

    [JsonPropertyName("client_status")]
    public IReadOnlyDictionary<Platform, UserStatus> Platform { get; init; }
}
