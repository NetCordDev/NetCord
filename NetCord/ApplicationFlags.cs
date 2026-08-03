using NetCord.Gateway;

namespace NetCord;

/// <summary>
/// Specifies the capabilities and status of an app.
/// </summary>
[Flags]
public enum ApplicationFlags : uint
{
    /// <summary>
    /// Indicates if an embedded app is available to play.
    /// </summary>
    EmbeddedReleased = 1 << 1,

    /// <summary>
    /// Indicates if an app has the ability to create twitch-style emoji.
    /// </summary>
    ManagedEmoji = 1 << 2,

    /// <summary>
    /// Indicates that an embedded app has the ability to create in-app purchases.
    /// </summary>
    EmbeddedInAppPurchases = 1 << 3,

    /// <summary>
    /// Indicates if an app has permission to create group DMs.
    /// </summary>
    GroupDMCreate = 1 << 4,

    /// <summary>
    /// Allows the application access the local RPC server.
    /// </summary>
    RpcPrivateBeta = 1 << 5,

    /// <summary>
    /// Indicates if an app uses the Auto Moderation API.
    /// </summary>
    ApplicationAutoModerationRuleCreateBadge = 1 << 6,

    /// <summary>
    /// Indicates if an app is allowed to create activity assets.
    /// </summary>
    AllowAssets = 1 << 8,

    /// <summary>
    /// Indicates if an app is allowed to enable activity spectating.
    /// </summary>
    AllowActivityActionSpectate = 1 << 9,

    /// <summary>
    /// Indicates if an app is allowed to enable join requests for activities.
    /// </summary>
    AllowActivityActionJoinRequest = 1 << 10,

    /// <summary>
    /// Indiicates whether an app has ever accessed the local RPC server.
    /// </summary>
    RPCHasConnected = 1 << 11,

    /// <summary>
    /// Intent required for bots in 100 or more servers to receive <see cref="GatewayClient.PresenceUpdate"/> events.
    /// </summary>
    GatewayPresence = 1 << 12,

    /// <summary>
    /// Intent required for bots in under 100 servers to receive <see cref="GatewayClient.PresenceUpdate"/> events, found on the Bot page in your app's settings.
    /// </summary>
    GatewayPresenceLimited = 1 << 13,

    /// <summary>
    /// Intent required for bots in 100 or more servers to receive guild user-related events like <see cref="GatewayClient.GuildUserAdd"/>.
    /// </summary>
    GatewayGuildUsers = 1 << 14,

    /// <summary>
    /// Intent required for bots in under 100 servers to receive guild user-related events like <see cref="GatewayClient.GuildUserAdd"/>, on the Bot page in your app's settings.
    /// </summary>
    GatewayGuildUsersLimited = 1 << 15,

    /// <summary>
    /// Indicates unusual growth of an app that prevents verification.
    /// </summary>
    VerificationPendingGuildLimit = 1 << 16,

    /// <summary>
    /// Indicates if an app is embedded within the Discord client.
    /// </summary>
    Embedded = 1 << 17,

    /// <summary>
    /// Intent required for bots in 100 or more servers to receive message content.
    /// </summary>
    GatewayMessageContent = 1 << 18,

    /// <summary>
    /// Intent required for bots in under 100 servers to receive message content, found on the Bot page in your app's settings.
    /// </summary>
    GatewayMessageContentLimited = 1 << 19,

    /// <summary>
    /// Indicates a first-party embedded app.
    /// </summary>
    EmbeddedFirstParty = 1 << 20,

    /// <summary>
    /// Indicates if an app has registered global application commands.
    /// </summary>
    ApplicationCommandBadge = 1 << 23,

    /// <summary>
    /// Indicates if an app is considered active.
    /// </summary>
    Active = 1 << 24,

    /// <summary>
    /// Indiciates if an app is allowed to use IFrame modals.
    /// </summary>
    IFrameModal = 1 << 26,
}
