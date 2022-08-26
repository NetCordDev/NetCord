using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonFollowedChannel : JsonEntity
{
    [JsonPropertyName("channel_id")]
    public override Snowflake Id { get; init; }

    [JsonPropertyName("webhook_id")]
    public Snowflake WebhookId { get; init; }
}
