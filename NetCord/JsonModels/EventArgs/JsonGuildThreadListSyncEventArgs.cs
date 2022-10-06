using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildThreadListSyncEventArgs
{
    [JsonPropertyName("guild_id")]
    public Snowflake GuildId { get; set; }

    [JsonPropertyName("channel_ids")]
    public Snowflake[]? ChannelIds { get; set; }

    [JsonPropertyName("threads")]
    public JsonChannel[] Threads { get; set; }

    [JsonPropertyName("members")]
    public JsonThreadUser[] Users { get; set; }

    [JsonSerializable(typeof(JsonGuildThreadListSyncEventArgs))]
    public partial class JsonGuildThreadListSyncEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildThreadListSyncEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
