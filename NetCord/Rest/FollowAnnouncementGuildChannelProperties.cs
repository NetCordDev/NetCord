using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class FollowAnnouncementGuildChannelProperties
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
    public ulong WebhookChannelId { get; set; }
}
