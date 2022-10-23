using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public partial class JsonIntegration : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public IntegrationType Type { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("syncing")]
    public bool? Syncing { get; set; }

    [JsonPropertyName("role_id")]
    public ulong? RoleId { get; set; }

    [JsonPropertyName("enable_emoticons")]
    public bool? EnableEmoticons { get; set; }

    [JsonPropertyName("expire_behavior")]
    public IntegrationExpireBehavior? ExpireBehavior { get; set; }

    [JsonPropertyName("expire_grace_period")]
    public int? ExpireGracePeriod { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? User { get; set; }

    [JsonPropertyName("account")]
    public JsonAccount Account { get; set; }

    [JsonPropertyName("synced_at")]
    public DateTimeOffset? SyncedAt { get; set; }

    [JsonPropertyName("subscriber_count")]
    public int? SubscriberCount { get; set; }

    [JsonPropertyName("revoked")]
    public bool? Revoked { get; set; }

    [JsonPropertyName("application")]
    public JsonIntegrationApplication? Application { get; set; }

    [JsonSerializable(typeof(JsonIntegration))]
    public partial class JsonIntegrationSerializerContext : JsonSerializerContext
    {
        public static JsonIntegrationSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }

    [JsonSerializable(typeof(JsonIntegration[]))]
    public partial class JsonIntegrationArraySerializerContext : JsonSerializerContext
    {
        public static JsonIntegrationArraySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
