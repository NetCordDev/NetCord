using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class StageInstanceProperties
{
    public StageInstanceProperties(ulong channelId, string topic)
    {
        ChannelId = channelId;
        Topic = topic;
    }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; }

    [JsonPropertyName("topic")]
    public string Topic { get; }

    [JsonPropertyName("privacy_level")]
    public StageInstancePrivacyLevel? PrivacyLevel { get; set; }

    [JsonPropertyName("send_start_notification")]
    public bool? SendStartNotification { get; set; }

    [JsonSerializable(typeof(StageInstanceProperties))]
    public partial class StageInstancePropertiesSerializerContext : JsonSerializerContext
    {
        public static StageInstancePropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
