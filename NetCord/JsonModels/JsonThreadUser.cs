using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonThreadUser : JsonThreadSelfUser
{
    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; init; }

    [JsonPropertyName("id")]
    public Snowflake ThreadId { get; init; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; init; }

    [JsonPropertyName("presence")]
    public JsonPresence? Presence { get; init; }
}
