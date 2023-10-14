namespace NetCord;

public class Entitlement : Entity, IJsonModel<JsonModels.JsonEntitlement>
{
    JsonModels.JsonEntitlement IJsonModel<JsonModels.JsonEntitlement>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEntitlement _jsonModel;

    public Entitlement(JsonModels.JsonEntitlement jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// Id of the SKU.
    /// </summary>
    public ulong SkuId => _jsonModel.SkuId;

    /// <summary>
    /// Id of the parent application.
    /// </summary>
    public ulong ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// Id of the user that is granted access to the entitlement's SKU.
    /// </summary>
    public ulong? UserId => _jsonModel.UserId;

    /// <summary>
    /// Type of the entitlement.
    /// </summary>
    public EntitlementType Type => _jsonModel.Type;

    /// <summary>
    /// Indicates whether the entitlement was deleted.
    /// </summary>
    public bool Deleted => _jsonModel.Deleted;

    /// <summary>
    /// Start date at which the entitlement is valid. Not present when using test entitlements.
    /// </summary>
    public DateTimeOffset? StartsAt => _jsonModel.StartsAt;

    /// <summary>
    /// Date at which the entitlement is no longer valid. Not present when using test entitlements.
    /// </summary>
    public DateTimeOffset? EndsAt => _jsonModel.EndsAt;

    /// <summary>
    /// Id of the guild that is granted access to the entitlement's SKU.
    /// </summary>
    public ulong? GuildId => _jsonModel.GuildId;
}
