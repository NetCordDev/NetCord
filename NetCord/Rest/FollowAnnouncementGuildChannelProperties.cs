using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Id of target channel.
/// </summary>
/// <param name="webhookChannelId"></param>
internal class FollowAnnouncementGuildChannelProperties(ulong webhookChannelId)
{

    /// <summary>
    /// Id of target channel.
    /// </summary>
    [JsonPropertyName("webhook_channel_id")]
    public ulong WebhookChannelId { get; set; } = webhookChannelId;
}
