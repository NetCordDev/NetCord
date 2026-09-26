using NetCord.JsonModels;

namespace NetCord;

public class Subscription(JsonSubscription jsonModel) : Entity, IJsonModel<JsonSubscription>
{
    JsonSubscription IJsonModel<JsonSubscription>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the subscription.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The ID of the user who is subscribed.
    /// </summary>
    public ulong UserId => jsonModel.UserId;

    /// <summary>
    /// The IDs of the SKUs subscribed to.
    /// </summary>
    public IReadOnlyList<ulong> SkuIds => jsonModel.SkuIds;

    /// <summary>
    /// The IDs of the entitlements granted for this subscription.
    /// </summary>
    public IReadOnlyList<ulong> EntitlementIds => jsonModel.EntitlementIds;

    /// <summary>
    /// The IDs of the SKUs that will be used for renewal.
    /// </summary>
    public IReadOnlyList<ulong>? RenewalSkuIds => jsonModel.RenewalSkuIds;

    /// <summary>
    /// The start of the current subscription period.
    /// </summary>
    public DateTimeOffset CurrentPeriodStart => jsonModel.CurrentPeriodStart;

    /// <summary>
    /// The end of the current subscription period.
    /// </summary>
    public DateTimeOffset CurrentPeriodEnd => jsonModel.CurrentPeriodEnd;

    /// <summary>
    /// The current status of the subscription.
    /// </summary>
    public SubscriptionStatus Status => jsonModel.Status;

    /// <summary>
    /// When the subscription was canceled.
    /// </summary>
    public DateTimeOffset? CanceledAt => jsonModel.CanceledAt;

    /// <summary>
    /// The country code of the payment source used to purchase the subscription. Missing unless queried with a private OAuth scope.
    /// </summary>
    public string? Country => jsonModel.Country;
}
