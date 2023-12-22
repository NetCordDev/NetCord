using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public class JsonGuildThreadUsersUpdateEventArgs
{
    [JsonPropertyName("id")]
    public ulong ThreadId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("member_count")]
    public int UserCount { get; set; }

    [JsonPropertyName("added_members")]
    public JsonThreadUser[]? AddedUsers { get; set; }

    [JsonPropertyName("removed_member_ids")]
    public ulong[] RemovedUserIds { get; set; }
}
