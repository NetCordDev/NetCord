namespace NetCord.Rest;

/// <summary>
/// Specifies additional information about an SKU's contents.
/// </summary>
[Flags]
public enum SkuFlags
{
    /// <summary>
    /// Whether the SKU is a premium purchase.
    /// </summary>
    PremiumPurchase = 1 << 0,

    /// <summary>
    /// Whether the SKU is free premium content.
    /// </summary>
    HasFreePremiumContent = 1 << 1,

    /// <summary>
    /// SKU is available for purchase.
    /// </summary>
    Available = 1 << 2,

    /// <summary>
    /// Whether the SKU is a premium or distribution product.
    /// </summary>
    PremiumAndDistribution = 1 << 3,

    /// <summary>
    /// Whether the SKU is a premium sticker pack.
    /// </summary>
    StickerPack = 1 << 4,

    /// <summary>
    /// Whether the SKU is a guild role subscription.
    /// </summary>
    GuildRoleSubscription = 1 << 5,

    /// <summary>
    /// Whether the SKU is a Discord premium subscription or related first-party product.
    /// </summary>
    PremiumSubscription = 1 << 6,

    /// <summary>
    /// Recurring SKU that can be purchased by a user and applied to a single server. Grants access to every user in that server.
    /// </summary>
    GuildSubscription = 1 << 7,

    /// <summary>
    /// Recurring SKU purchased by a user for themselves. Grants access to the purchasing user in every server.
    /// </summary>
    UserSubscription = 1 << 8,
}
