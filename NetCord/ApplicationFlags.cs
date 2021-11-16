namespace NetCord;

public enum ApplicationFlags
{
    GatewayPresence = 1 << 12,
    GatewayPresenceLimited = 1 << 13,
    GatewayGuildMembers = 1 << 14,
    GatewayGuildMembersLimited = 1 << 15,
    VerificationPendingGuildLimit = 1 << 16,
    Embedded = 1 << 17,
}
