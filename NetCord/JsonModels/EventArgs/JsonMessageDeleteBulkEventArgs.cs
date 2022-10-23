using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonMessageDeleteBulkEventArgs
{
    [JsonPropertyName("ids")]
    public ulong[] MessageIds { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonSerializable(typeof(JsonMessageDeleteBulkEventArgs))]
    public partial class JsonMessageDeleteBulkEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageDeleteBulkEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
