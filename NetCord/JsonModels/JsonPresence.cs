using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonPresence
{
    [JsonPropertyName("user")]
    public JsonUser User { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("status")]
    public UserStatusType Status { get; init; }

    [JsonPropertyName("activities")]
    public JsonUserActivity[] Activities { get; init; }

    [JsonPropertyName("client_status")]
    public IReadOnlyDictionary<Platform, UserStatusType> Platform { get; init; }
}
