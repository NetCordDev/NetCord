using NetCord.JsonModels;

namespace NetCord;

public class Subscription(JsonSubscription jsonModel) : Entity(jsonModel)
{
    /// <summary>
    /// The ID of the user who is subscribed.
    /// </summary>
    public ulong UserId { get; } = jsonModel.UserId;

    /// <summary>
    /// The IDs of the SKUs subscribed to.
    /// </summary>
    public IReadOnlyList<ulong> SkuIds { get; } = jsonModel.SkuIds;

    /// <summary>
    /// The IDs of the entitlements granted for this subscription.
    /// </summary>
    public IReadOnlyList<ulong> EntitlementIds { get; } = jsonModel.EntitlementIds;

    /// <summary>
    /// The IDs of the SKUs that will be used for renewal.
    /// </summary>
    public IReadOnlyList<ulong>? RenewalSkuIds { get; } = jsonModel.RenewalSkuIds;

    /// <summary>
    /// The start of the current subscription period.
    /// </summary>
    public DateTimeOffset CurrentPeriodStart { get; } = jsonModel.CurrentPeriodStart;

    /// <summary>
    /// The end of the current subscription period.
    /// </summary>
    public DateTimeOffset CurrentPeriodEnd { get; } = jsonModel.CurrentPeriodEnd;

    /// <summary>
    /// The current status of the subscription.
    /// </summary>
    public SubscriptionStatus Status { get; } = jsonModel.Status;

    /// <summary>
    /// When the subscription was canceled.
    /// </summary>
    public DateTimeOffset? CanceledAt { get; } = jsonModel.CanceledAt;

    /// <summary>
    /// The country code of the payment source used to purchase the subscription. Missing unless queried with a private OAuth scope.
    /// </summary>
    public string? Country { get; } = jsonModel.Country;
}
