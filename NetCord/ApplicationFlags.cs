using NetCord.Gateway;

namespace NetCord;

[Flags]
public enum ApplicationFlags : uint
{
    /// <summary>
    /// Undocumented.
    /// </summary>
    EmbeddedReleased = 1 << 1,

    /// <summary>
    /// Undocumented.
    /// </summary>
    ManagedEmoji = 1 << 2,

    /// <summary>
    /// Undocumented.
    /// </summary>
    EmbeddedIAP = 1 << 3,

    /// <summary>
    /// Undocumented.
    /// </summary>
    GroupDMCreate = 1 << 4,

    /// <summary>
    /// Indicates if an app uses the Auto Moderation API.
    /// </summary>
    ApplicationAutoModerationRuleCreateBadge = 1 << 6,

    /// <summary>
    /// Undocumented.
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
    /// Undocumented.
    /// </summary>
    EmbeddedFirstParty = 1 << 20,

    /// <summary>
    /// Indicates if an app has registered global application commands.
    /// </summary>
    ApplicationCommandBadge = 1 << 23,
}
