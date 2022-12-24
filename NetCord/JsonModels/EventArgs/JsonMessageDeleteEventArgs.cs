using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonMessageDeleteEventArgs
{
    [JsonPropertyName("id")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonSerializable(typeof(JsonMessageDeleteEventArgs))]
    public partial class JsonMessageDeleteEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonMessageDeleteEventArgsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
