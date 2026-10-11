namespace NetCord;

/// <summary>
/// Contains information about a role subscribed to by a user.
/// </summary>
public class RoleSubscriptionData(JsonModels.JsonRoleSubscriptionData jsonModel)
{
    /// <summary>
    /// The ID of the SKU and listing that the user is subscribed to.
    /// </summary>
    public ulong RoleSubscriptionListingId { get; } = jsonModel.RoleSubscriptionListingId;

    /// <summary>
    /// The name of the tier that the user is subscribed to.
    /// </summary>
    public string TierName { get; } = jsonModel.TierName;

    /// <summary>
    /// The cumulative number of months that the user has been subscribed for.
    /// </summary>
    public int TotalMonthsSubscribed { get; } = jsonModel.TotalMonthsSubscribed;

    /// <summary>
    /// Whether this notification is for a renewal rather than a new purchase.
    /// </summary>
    public bool IsRenewal { get; } = jsonModel.IsRenewal;
}
