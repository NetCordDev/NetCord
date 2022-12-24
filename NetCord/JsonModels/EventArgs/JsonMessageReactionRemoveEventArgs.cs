using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonMessageReactionRemoveEventArgs
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; set; }

    [JsonSerializable(typeof(JsonMessageReactionRemoveEventArgs))]
    public partial class JsonMessageReactionRemoveEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageReactionRemoveEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
