using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonGuildThreadListSyncEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_ids")]
    public ulong[]? ChannelIds { get; set; }

    [JsonPropertyName("threads")]
    public JsonChannel[] Threads { get; set; }

    [JsonPropertyName("members")]
    public JsonThreadUser[] Users { get; set; }

    [JsonSerializable(typeof(JsonGuildThreadListSyncEventArgs))]
    public partial class JsonGuildThreadListSyncEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonGuildThreadListSyncEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
