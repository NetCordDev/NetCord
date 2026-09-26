using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonSubscription : JsonEntity
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("sku_ids")]
    public ulong[] SkuIds { get; set; }

    [JsonPropertyName("entitlement_ids")]
    public ulong[] EntitlementIds { get; set; }

    [JsonPropertyName("renewal_sku_ids")]
    public ulong[]? RenewalSkuIds { get; set; }

    [JsonPropertyName("current_period_start")]
    public DateTimeOffset CurrentPeriodStart { get; set; }

    [JsonPropertyName("current_period_end")]
    public DateTimeOffset CurrentPeriodEnd { get; set; }

    [JsonPropertyName("status")]
    public SubscriptionStatus Status { get; set; }

    [JsonPropertyName("canceled_at")]
    public DateTimeOffset? CanceledAt { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}
