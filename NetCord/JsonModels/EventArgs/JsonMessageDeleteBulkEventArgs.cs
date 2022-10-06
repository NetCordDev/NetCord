using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonMessageDeleteBulkEventArgs
{
    [JsonPropertyName("ids")]
    public Snowflake[] MessageIds { get; set; }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; set; }

    [JsonSerializable(typeof(JsonMessageDeleteBulkEventArgs))]
    public partial class JsonMessageDeleteBulkEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageDeleteBulkEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
