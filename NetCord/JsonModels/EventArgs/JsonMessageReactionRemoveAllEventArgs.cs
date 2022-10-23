using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonMessageReactionRemoveAllEventArgs
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonSerializable(typeof(JsonMessageReactionRemoveAllEventArgs))]
    public partial class JsonMessageReactionRemoveAllEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageReactionRemoveAllEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
