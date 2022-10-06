using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonMessageReactionAddEventArgs
{
    [JsonPropertyName("user_id")]
    public Snowflake UserId { get; set; }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public Snowflake MessageId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? User { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; set; }

    [JsonSerializable(typeof(JsonMessageReactionAddEventArgs))]
    public partial class JsonMessageReactionAddEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageReactionAddEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
