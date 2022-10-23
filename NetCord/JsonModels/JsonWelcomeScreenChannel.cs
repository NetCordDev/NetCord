using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonWelcomeScreenChannel
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("emoji_id")]
    public ulong? EmojiId { get; set; }

    [JsonPropertyName("emoji_name")]
    public string? EmojiName { get; set; }

    [JsonSerializable(typeof(JsonWelcomeScreenChannel))]
    public partial class JsonWelcomeScreenChannelSerializerContext : JsonSerializerContext
    {
        public static JsonWelcomeScreenChannelSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
