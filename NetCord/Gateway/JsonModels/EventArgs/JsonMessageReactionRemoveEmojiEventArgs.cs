using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Gateway.JsonModels.EventArgs;

public partial class JsonMessageReactionRemoveEmojiEventArgs
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; set; }

    [JsonSerializable(typeof(JsonMessageReactionRemoveEmojiEventArgs))]
    public partial class JsonMessageReactionRemoveEmojiEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageReactionRemoveEmojiEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
