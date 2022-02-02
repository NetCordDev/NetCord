using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonFollowedChannel : JsonEntity
{
    [JsonPropertyName("channel_id")]
    public override DiscordId Id { get; init; }

    [JsonPropertyName("webhook_id")]
    public DiscordId WebhookId { get; init; }
}