using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

internal record JsonGuildThreadMembersUpdateEventArgs
{
    [JsonPropertyName("id")]
    public Snowflake ThreadId { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; init; }

    [JsonPropertyName("member_count")]
    public int MemberCount { get; init; }

    // In this gateway event, the thread member objects will also include the guild member and nullable presence objects for each added thread member.
    [JsonPropertyName("added_members")]
    public JsonThreadUser[]? AddedUsers { get; init; }

    [JsonPropertyName("removed_member_ids")]
    public Snowflake[] RemovedUserIds { get; init; }
}
