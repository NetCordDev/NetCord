using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonChannelPinsUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("last_pin_timestamp")]
    public DateTimeOffset? LastPinTimestamp { get; set; }

    [JsonSerializable(typeof(JsonChannelPinsUpdateEventArgs))]
    public partial class JsonChannelPinsUpdateEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonChannelPinsUpdateEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
