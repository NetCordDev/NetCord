using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal partial class FollowAnnouncementGuildChannelProperties
{
    /// <summary>
    /// Id of target channel.
    /// </summary>
    /// <param name="webhookChannelId"></param>
    public FollowAnnouncementGuildChannelProperties(ulong webhookChannelId)
    {
        WebhookChannelId = webhookChannelId;
    }

    /// <summary>
    /// Id of target channel.
    /// </summary>
    [JsonPropertyName("webhook_channel_id")]
    public ulong WebhookChannelId { get; }

    [JsonSerializable(typeof(FollowAnnouncementGuildChannelProperties))]
    public partial class FollowAnnouncementGuildChannelPropertiesSerializerContext : JsonSerializerContext
    {
        public static FollowAnnouncementGuildChannelPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
