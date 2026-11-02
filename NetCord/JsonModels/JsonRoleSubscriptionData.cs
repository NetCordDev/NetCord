using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonRoleSubscriptionData
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
