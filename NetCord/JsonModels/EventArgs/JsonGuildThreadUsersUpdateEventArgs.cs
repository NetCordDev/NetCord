using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildThreadUsersUpdateEventArgs
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

    [JsonSerializable(typeof(JsonGuildThreadUsersUpdateEventArgs))]
    public partial class JsonGuildThreadUsersUpdateEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildThreadUsersUpdateEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
