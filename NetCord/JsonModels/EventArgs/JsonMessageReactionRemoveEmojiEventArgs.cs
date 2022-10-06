using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonMessageReactionRemoveEmojiEventArgs
{
    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; set; }

    [JsonPropertyName("message_id")]
    public Snowflake MessageId { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; set; }

    [JsonSerializable(typeof(JsonMessageReactionRemoveEmojiEventArgs))]
    public partial class JsonMessageReactionRemoveEmojiEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageReactionRemoveEmojiEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
