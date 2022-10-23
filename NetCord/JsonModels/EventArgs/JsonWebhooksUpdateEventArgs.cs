using System.Text.Json.Serialization;

namespace NetCord.JsonModels.EventArgs;

public partial class JsonWebhooksUpdateEventArgs
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonSerializable(typeof(JsonWebhooksUpdateEventArgs))]
    public partial class JsonWebhooksUpdateEventArgsSerializerContext : JsonSerializerContext
    {
        public static JsonWebhooksUpdateEventArgsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
