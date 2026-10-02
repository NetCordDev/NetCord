using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonSubscription : JsonEntity
{
    [JsonPropertyName("user_id")]
    public required ulong UserId { get; set; }

    [JsonPropertyName("sku_ids")]
    public required ulong[] SkuIds { get; set; }

    [JsonPropertyName("entitlement_ids")]
    public required ulong[] EntitlementIds { get; set; }

    [JsonPropertyName("renewal_sku_ids")]
    public ulong[]? RenewalSkuIds { get; set; }

    [JsonPropertyName("current_period_start")]
    public required DateTimeOffset CurrentPeriodStart { get; set; }

    [JsonPropertyName("current_period_end")]
    public required DateTimeOffset CurrentPeriodEnd { get; set; }

    [JsonPropertyName("status")]
    public required SubscriptionStatus Status { get; set; }

    [JsonPropertyName("canceled_at")]
    public DateTimeOffset? CanceledAt { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}
