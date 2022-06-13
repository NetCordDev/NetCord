using System.Text.Json.Serialization;

namespace NetCord;

public class StageInstanceProperties
{
    public StageInstanceProperties(Snowflake channelId, string topic)
    {
        ChannelId = channelId;
        Topic = topic;
    }

    [JsonPropertyName("channel_id")]
    public Snowflake ChannelId { get; }

    [JsonPropertyName("topic")]
    public string Topic { get; }

    [JsonPropertyName("privacy_level")]
    public StageInstancePrivacyLevel? PrivacyLevel { get; set; }

    [JsonPropertyName("send_start_notification")]
    public bool? SendStartNotification { get; set; }
}