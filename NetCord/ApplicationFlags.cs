namespace NetCord;

[Flags]
public enum ApplicationFlags : uint
{
    GatewayPresence = 1 << 12,
    GatewayPresenceLimited = 1 << 13,
    GatewayGuildUsers = 1 << 14,
    GatewayGuildUsersLimited = 1 << 15,
    VerificationPendingGuildLimit = 1 << 16,
    Embedded = 1 << 17,
    GatewayMessageContent = 1 << 18,
    GatewayMessageContentLimited = 1 << 19,
    ApplicationCommandBadge = 1 << 23,
}