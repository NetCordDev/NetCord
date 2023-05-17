namespace NetCord;

public class RoleSubscriptionData : IJsonModel<JsonModels.JsonRoleSubscriptionData>
{
    JsonModels.JsonRoleSubscriptionData IJsonModel<JsonModels.JsonRoleSubscriptionData>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonRoleSubscriptionData _jsonModel;

    public RoleSubscriptionData(JsonModels.JsonRoleSubscriptionData jsonModel)
    {
        _jsonModel = jsonModel;
    }

    /// <summary>
    /// The id of the sku and listing that the user is subscribed to.
    /// </summary>
    public ulong RoleSubscriptionListingId => _jsonModel.RoleSubscriptionListingId;

    /// <summary>
    /// The name of the tier that the user is subscribed to.
    /// </summary>
    public string TierName => _jsonModel.TierName;

    /// <summary>
    /// The cumulative number of months that the user has been subscribed for.
    /// </summary>
    public int TotalMonthsSubscribed => _jsonModel.TotalMonthsSubscribed;

    /// <summary>
    /// Whether this notification is for a renewal rather than a new purchase.
    /// </summary>
    public bool IsRenewal => _jsonModel.IsRenewal;
}
