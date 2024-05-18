using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// ID of target channel.
/// </summary>
/// <param name="webhookChannelId"></param>
internal class FollowAnnouncementGuildChannelProperties(ulong webhookChannelId)
{
    /// <summary>
    /// ID of target channel.
    /// </summary>
    [JsonPropertyName("webhook_channel_id")]
    public ulong WebhookChannelId { get; set; } = webhookChannelId;
}
