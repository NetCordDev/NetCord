using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class WebhookOptions
{
    internal WebhookOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar")]
    public ImageProperties? Avatar { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonSerializable(typeof(WebhookOptions))]
    public partial class WebhookOptionsSerializerContext : JsonSerializerContext
    {
        public static WebhookOptionsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
