using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class JsonMessageReference
{
    [JsonPropertyName("message_id")]
    public ulong? MessageId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("fail_if_not_exists")]
    public bool? FailIfNotExists { get; set; }

    [JsonSerializable(typeof(JsonMessageReference))]
    public partial class JsonMessageReferenceSerializerContext : JsonSerializerContext
    {
        public static JsonMessageReferenceSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
