using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class JsonMessageReference
{
    [JsonPropertyName("message_id")]
    public Snowflake? MessageId { get; set; }

    [JsonPropertyName("channel_id")]
    public Snowflake? ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; set; }

    [JsonPropertyName("fail_if_not_exists")]
    public bool? FailIfNotExists { get; set; }

    [JsonSerializable(typeof(JsonMessageReference))]
    public partial class JsonMessageReferenceSerializerContext : JsonSerializerContext
    {
        public static JsonMessageReferenceSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
