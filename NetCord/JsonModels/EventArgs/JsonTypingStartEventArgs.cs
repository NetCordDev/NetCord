using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonTypingStartEventArgs
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonConverter(typeof(JsonConverters.SecondsUnixDateTimeOffsetConverter))]
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? User { get; set; }

    [JsonSerializable(typeof(JsonTypingStartEventArgs))]
    public partial class JsonTypingStartEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonTypingStartEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
