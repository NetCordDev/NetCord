using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonIntegration : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("type")]
    public IntegrationType Type { get; init; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; }

    [JsonPropertyName("syncing")]
    public bool? Syncing { get; init; }

    [JsonPropertyName("role_id")]
    public Snowflake? RoleId { get; init; }

    [JsonPropertyName("enable_emoticons")]
    public bool? EnableEmoticons { get; init; }

    [JsonPropertyName("expire_behavior")]
    public IntegrationExpireBehavior? ExpireBehavior { get; init; }

    [JsonPropertyName("expire_grace_period")]
    public int? ExpireGracePeriod { get; init; }

    [JsonPropertyName("user")]
    public JsonUser? User { get; init; }

    [JsonPropertyName("account")]
    public JsonAccount Account { get; init; }

    [JsonPropertyName("synced_at")]
    public DateTimeOffset? SyncedAt { get; init; }

    [JsonPropertyName("subscriber_count")]
    public int? SubscriberCount { get; init; }

    [JsonPropertyName("revoked")]
    public bool? Revoked { get; init; }

    [JsonPropertyName("application")]
    public JsonIntegrationApplication? Application { get; init; }
}