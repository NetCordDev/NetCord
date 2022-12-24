using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal partial class FollowAnnouncementGuildChannelProperties
{
    public FollowAnnouncementGuildChannelProperties(ulong webhookChannelId)
    {
        WebhookChannelId = webhookChannelId;
    }

    [JsonPropertyName("webhook_channel_id")]
    public ulong WebhookChannelId { get; }

    [JsonSerializable(typeof(FollowAnnouncementGuildChannelProperties))]
    public partial class FollowAnnouncementGuildChannelPropertiesSerializerContext : JsonSerializerContext
    {
        public static FollowAnnouncementGuildChannelPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
