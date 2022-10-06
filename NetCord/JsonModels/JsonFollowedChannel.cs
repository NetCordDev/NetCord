using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonFollowedChannel : JsonEntity
{
    [JsonPropertyName("channel_id")]
    public override Snowflake Id { get; set; }

    [JsonPropertyName("webhook_id")]
    public Snowflake WebhookId { get; set; }

    [JsonSerializable(typeof(JsonFollowedChannel))]
    public partial class JsonFollowedChannelSerializerContext : JsonSerializerContext
    {
        public static JsonFollowedChannelSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
