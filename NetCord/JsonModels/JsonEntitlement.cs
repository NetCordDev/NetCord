using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEntitlement : JsonEntity
{
    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("user_id")]
    public ulong? UserId { get; set; }

    [JsonPropertyName("type")]
    public EntitlementType Type { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }

    [JsonPropertyName("starts_at")]
    public DateTimeOffset? StartsAt { get; set; }

    [JsonPropertyName("ends_at")]
    public DateTimeOffset? EndsAt { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonSerializable(typeof(JsonEntitlement))]
    public partial class JsonEntitlementSerializerContext : JsonSerializerContext
    {
        public static JsonEntitlementSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(JsonEntitlement[]))]
    public partial class JsonEntitlementArraySerializerContext : JsonSerializerContext
    {
        public static JsonEntitlementArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
