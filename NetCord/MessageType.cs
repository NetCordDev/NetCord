namespace NetCord;

public enum MessageType
{
    /// <summary>
    /// The default message type.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Sent when a recipient is added.
    /// </summary>
    RecipientAdd = 1,

    /// <summary>
    /// Sent when a recipient is removed.
    /// </summary>
    RecipientRemove = 2,

    /// <summary>
    /// Sent when a user is called.
    /// </summary>
    Call = 3,

    /// <summary>
    /// Sent when a Group DM's name is changed.
    /// </summary>
    NameChange = 4,

    /// <summary>
    /// Sent when a Group DM's icon is changed.
    /// </summary>
    IconChange = 5,

    /// <summary>
    /// Sent when a message is pinned to a channel.
    /// </summary>
    MessagePinned = 6,

    /// <summary>
    /// Sent when a member joins a guild.
    /// </summary>
    GuildMembedJoined = 7,

    /// <summary>
    /// Sent when a user boosts a guild.
    /// </summary>
    PremiumGuildSubscription = 8,

    /// <summary>
    /// Send when a guild reaches boost level 1.
    /// </summary>
    PremiumGuildSubscriptionTier1 = 9,

    /// <summary>
    /// Send when a guild reaches boost level 2.
    /// </summary>
    PremiumGuildSubscriptionTier2 = 10,

    /// <summary>
    /// Send when a guild reaches boost level 3.
    /// </summary>
    PremiumGuildSubscriptionTier3 = 11,

    /// <summary>
    /// Sent when a channel subscription is added to another channel.
    /// </summary>
    ChannelFollowAdd = 12,

    /// <summary>
    /// Sent when a guild is disqualified from Server Discovery.
    /// </summary>
    GuildDiscoveryDisqualified = 14,

    /// <summary>
    /// Sent when a guild is requalified for Server Discovery.
    /// </summary>
    GuildDiscoveryRequalified = 15,

    /// <summary>
    /// Sent as intial warning during the grace period for Server Discovery disqualification.
    /// </summary>
    GuildDiscoveryGracePeriodInitialWarning = 16,

    /// <summary>
    /// Sent as final warning during the grace period for Server Discovery disqualification.
    /// </summary>
    GuildDiscoveryGracePeriodFinalWarning = 17,

    /// <summary>
    /// Sent when a thread is created.
    /// </summary>
    ThreadCreated = 18,

    /// <summary>
    /// Sent when a message is a reply to another message.
    /// </summary>
    Reply = 19,

    /// <summary>
    /// Sent when an application command is triggered.
    /// </summary>
    ApplicationCommand = 20,

    /// <summary>
    /// Sent as the initial message in a thread.
    /// </summary>
    ThreadStarterMessage = 21,

    /// <summary>
    /// Sent as a reminder to invite other users to a guild.
    /// </summary>
    GuildInviteReminder = 22,

    /// <summary>
    /// Sent when a context menu command is triggered.
    /// </summary>
    ContextMenuCommand = 23,

    /// <summary>
    /// Sent when an automod action is taken.
    /// </summary>
    AutoModerationAction = 24,

    /// <summary>
    /// Sent when a subscription to a role is purchased.
    /// </summary>
    RoleSubscriptionPurchase = 25,

    /// <summary>
    /// Sent when a premium upsell for an interaction is triggered.
    /// </summary>
    InteractionPremiumUpsell = 26,

    /// <summary>
    /// Sent when a stage is started.
    /// </summary>
    StageStart = 27,

    /// <summary>
    /// Sent when a stage ends.
    /// </summary>
    StageEnd = 28,

    /// <summary>
    /// Sent when a user joins a stage as a speaker.
    /// </summary>
    StageSpeaker = 29,

    /// <summary>
    /// Sent when a user raises their hand in a stage.
    /// </summary>
    StageRaiseHand = 30,

    /// <summary>
    /// Sent alongside <see cref="StageRaiseHand"/>.
    /// </summary>
    StageTopic = 31,

    /// <summary>
    /// Sent for premium subscriptions to a guild application.
    /// </summary>
    GuildApplicationPremiumSubscription = 32,

    /// <summary>
    /// Sent when incident alert mode is enabled in a guild.
    /// </summary>
    IncidentAlertModeEnabled = 36,

    /// <summary>
    /// Sent when incident alert mode is disabled in a guild.
    /// </summary>
    IncidentAlertModeDisabled = 37,

    /// <summary>
    /// Sent when a raid incident is reported in a guild.
    /// </summary>
    IncidentReportRaid = 38,

    /// <summary>
    /// Sent when a false alarm incident is reported in a guild.
    /// </summary>
    IncidentReportFalseAlarm = 39,

    /// <summary>
    /// Sent when a purchase is made in a guild.
    /// </summary>
    PurchaseNotification = 44
}
