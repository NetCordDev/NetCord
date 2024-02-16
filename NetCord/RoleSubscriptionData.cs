namespace NetCord;

public class RoleSubscriptionData(JsonModels.JsonRoleSubscriptionData jsonModel) : IJsonModel<JsonModels.JsonRoleSubscriptionData>
{
    JsonModels.JsonRoleSubscriptionData IJsonModel<JsonModels.JsonRoleSubscriptionData>.JsonModel => jsonModel;

    /// <summary>
    /// The id of the sku and listing that the user is subscribed to.
    /// </summary>
    public ulong RoleSubscriptionListingId => jsonModel.RoleSubscriptionListingId;

    /// <summary>
    /// The name of the tier that the user is subscribed to.
    /// </summary>
    public string TierName => jsonModel.TierName;

    /// <summary>
    /// The cumulative number of months that the user has been subscribed for.
    /// </summary>
    public int TotalMonthsSubscribed => jsonModel.TotalMonthsSubscribed;

    /// <summary>
    /// Whether this notification is for a renewal rather than a new purchase.
    /// </summary>
    public bool IsRenewal => jsonModel.IsRenewal;
}
