using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonRoleSubscriptionData
{
    [JsonPropertyName("role_subscription_listing_id")]
    public required ulong RoleSubscriptionListingId { get; set; }

    [JsonPropertyName("tier_name")]
    public required string TierName { get; set; }

    [JsonPropertyName("total_months_subscribed")]
    public required int TotalMonthsSubscribed { get; set; }

    [JsonPropertyName("is_renewal")]
    public required bool IsRenewal { get; set; }
}
