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
    public ulong ChannelId { get; set; }

    [JsonPropertyName("topic")]
    public string Topic { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("privacy_level")]
    public StageInstancePrivacyLevel? PrivacyLevel { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("send_start_notification")]
    public bool? SendStartNotification { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong? GuildScheduledEventId { get; set; }
}
