using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public record JsonGuildThreadUsersUpdateEventArgs
{
    [JsonPropertyName("id")]
    public Snowflake ThreadId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("member_count")]
    public int UserCount { get; init; }

    [JsonPropertyName("added_members")]
    public JsonThreadUser[]? AddedUsers { get; init; }

    [JsonPropertyName("removed_member_ids")]
    public Snowflake[] RemovedUserIds { get; init; }
}
