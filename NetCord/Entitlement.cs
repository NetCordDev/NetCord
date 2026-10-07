using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class Entitlement(JsonEntitlement jsonModel, RestClient client) : ClientEntity(jsonModel, client)
{
    /// <summary>
    /// ID of the SKU.
    /// </summary>
    public ulong SkuId { get; } = jsonModel.SkuId;

    /// <summary>
    /// ID of the parent application.
    /// </summary>
    public ulong ApplicationId { get; } = jsonModel.ApplicationId;

    /// <summary>
    /// ID of the user that is granted access to the entitlement's SKU.
    /// </summary>
    public ulong? UserId { get; } = jsonModel.UserId;

    /// <summary>
    /// Type of the entitlement.
    /// </summary>
    public EntitlementType Type { get; } = jsonModel.Type;

    /// <summary>
    /// Indicates whether the entitlement was deleted.
    /// </summary>
    public bool Deleted { get; } = jsonModel.Deleted;

    /// <summary>
    /// Start date at which the entitlement is valid. Not present when using test entitlements.
    /// </summary>
    public DateTimeOffset? StartsAt { get; } = jsonModel.StartsAt;

    /// <summary>
    /// Date at which the entitlement is no longer valid. Not present when using test entitlements.
    /// </summary>
    public DateTimeOffset? EndsAt { get; } = jsonModel.EndsAt;

    /// <summary>
    /// ID of the guild that is granted access to the entitlement's SKU.
    /// </summary>
    public ulong? GuildId { get; } = jsonModel.GuildId;

    /// <summary>
    /// For consumable items, whether or not the entitlement has been consumed.
    /// </summary>
    public bool? Consumed { get; } = jsonModel.Consumed;
}
