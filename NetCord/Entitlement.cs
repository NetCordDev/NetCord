namespace NetCord;

public class Entitlement(JsonModels.JsonEntitlement jsonModel) : Entity, IJsonModel<JsonModels.JsonEntitlement>
{
    JsonModels.JsonEntitlement IJsonModel<JsonModels.JsonEntitlement>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// ID of the SKU.
    /// </summary>
    public ulong SkuId => jsonModel.SkuId;

    /// <summary>
    /// ID of the parent application.
    /// </summary>
    public ulong ApplicationId => jsonModel.ApplicationId;

    /// <summary>
    /// ID of the user that is granted access to the entitlement's SKU.
    /// </summary>
    public ulong? UserId => jsonModel.UserId;

    /// <summary>
    /// Type of the entitlement.
    /// </summary>
    public EntitlementType Type => jsonModel.Type;

    /// <summary>
    /// Indicates whether the entitlement was deleted.
    /// </summary>
    public bool Deleted => jsonModel.Deleted;

    /// <summary>
    /// Start date at which the entitlement is valid. Not present when using test entitlements.
    /// </summary>
    public DateTimeOffset? StartsAt => jsonModel.StartsAt;

    /// <summary>
    /// Date at which the entitlement is no longer valid. Not present when using test entitlements.
    /// </summary>
    public DateTimeOffset? EndsAt => jsonModel.EndsAt;

    /// <summary>
    /// ID of the guild that is granted access to the entitlement's SKU.
    /// </summary>
    public ulong? GuildId => jsonModel.GuildId;

    /// <summary>
    /// For consumable items, whether or not the entitlement has been consumed.
    /// </summary>
    public bool? Consumed => jsonModel.Consumed;
}
