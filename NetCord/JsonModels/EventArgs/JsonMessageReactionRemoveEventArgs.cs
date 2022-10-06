using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonMessageReactionRemoveEventArgs
{
    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; set; }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public Snowflake MessageId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; set; }

    [JsonSerializable(typeof(JsonMessageReactionRemoveEventArgs))]
    public partial class JsonMessageReactionRemoveEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageReactionRemoveEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
