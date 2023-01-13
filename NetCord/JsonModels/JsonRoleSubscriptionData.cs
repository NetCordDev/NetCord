using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonRoleSubscriptionData
{
    [JsonPropertyName("role_subscription_listing_id")]
    public ulong RoleSubscriptionListingId { get; set; }

    [JsonPropertyName("tier_name")]
    public string TierName { get; set; }

    [JsonPropertyName("total_months_subscribed")]
    public int TotalMonthsSubscribed { get; set; }

    [JsonPropertyName("is_renewal")]
    public bool IsRenewal { get; set; }
}
