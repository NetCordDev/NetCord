using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonMessageReactionRemoveAllEventArgs
{
    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public Snowflake MessageId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; set; }

    [JsonSerializable(typeof(JsonMessageReactionRemoveAllEventArgs))]
    public partial class JsonMessageReactionRemoveAllEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageReactionRemoveAllEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
