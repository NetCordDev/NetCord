using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonMessageDeleteEventArgs
{
    [JsonPropertyName("id")]
    public Snowflake MessageId { get; set; }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; set; }

    [JsonSerializable(typeof(JsonMessageDeleteEventArgs))]
    public partial class JsonMessageDeleteEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageDeleteEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
