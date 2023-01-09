namespace NetCord;

public enum MessageType
{
    Default = 0,
    RecipientAdd = 1,
    RecipientRemove = 2,
    Call = 3,
    /// <summary>
    /// Group dm only
    /// </summary>
    NameChange = 4,
    /// <summary>
    /// Group dm only
    /// </summary>
    IconChange = 5,
    MessagePinned = 6,
    GuildMembedJoined = 7,
    PremiumGuildSubscription = 8,
    PremiumGuildSubscriptionTier1 = 9,
    PremiumGuildSubscriptionTier2 = 10,
    PremiumGuildSubscriptionTier3 = 11,
    ChannelFollowAdd = 12,
    GuildDiscoveryDisqualified = 14,
    GuildDiscoveryRequalified = 15,
    GuildDiscoveryGracePeriodInitialWarning = 16,
    GuildDiscoveryGracePeriodFinalWarning = 17,
    ThreadCreated = 18,
    Reply = 19,
    ApplicationCommand = 20,
    ThreadStarterMessage = 21,
    GuildInviteReminder = 22,
    ContextMenuCommand = 23,
    AutoModerationAction = 24,
    RoleSubscriptionPurchase = 25,
}
