namespace NetCord;

[Flags]
public enum ApplicationFlags : uint
{
    GatewayPresence = 1 << 12,
    GatewayPresenceLimited = 1 << 13,
    GatewayGuildMembers = 1 << 14,
    GatewayGuildMembersLimited = 1 << 15,
    VerificationPendingGuildLimit = 1 << 16,
    Embedded = 1 << 17,
    GatewayMessageContent = 1 << 18,
    GatewayMessageContentLimited = 1 << 19,
}