namespace NetCord;

[Flags]
public enum UserFlags : ulong
{
    /// <summary>
    /// User is a Discord employee.
    /// </summary>
    Staff = 1uL << 0,

    /// <summary>
    /// User is a Discord partner.
    /// </summary>
    Partner = 1uL << 1,

    /// <summary>
    /// User has the 'HypeSquad Events' badge.
    /// </summary>
    HypeSquad = 1uL << 2,

    /// <summary>
    /// User has the 'Bug Hunter' badge.
    /// </summary>
    BugHunterLevel1 = 1uL << 3,

    /// <summary>
    /// Undocumented and Private. User has SMS recovery for 2FA enabled.
    /// </summary>
    MfaSms = 1uL << 4,

    /// <summary>
    /// Undocumented and Private. User dismissed the Nitro promotion.
    /// </summary>
    PremiumPromoDismissed = 1uL << 5,

    // HypeSquad House Flags

    /// <summary>
    /// User is a House of Bravery Member.
    /// </summary>
    HypeSquadOnlineHouse1 = 1uL << 6,

    /// <summary>
    /// User is a House of Brilliance Member.
    /// </summary>
    HypeSquadOnlineHouse = 1uL << 7,

    /// <summary>
    /// User is a House of Balance Member.
    /// </summary>
    HypeSquadOnlineHouse3 = 1uL << 8,

    /// <summary>
    /// User has the 'Early Supporter' badge.
    /// </summary>
    PremiumEarlySupporter = 1uL << 9,

    /// <summary>
    /// User is a team. See <see href="https://discord.com/developers/docs/topics/teams"/>.
    /// </summary>
    TeamPseudoUser = 1uL << 10,

    /// <summary>
    /// Undocumented and Private. User has a pending partner/verification application.
    /// </summary>
    InternalApplication = 1uL << 11,

    /// <summary>
    /// Undocumented. User is the SYSTEM account.
    /// </summary>
    System = 1uL << 12,

    /// <summary>
    /// Undocumented and Private. User has unread messages from Discord.
    /// </summary>
    HasUnreadUrgentMessages = 1uL << 13,

    /// <summary>
    /// User has the 'Golden Bug Hunter' badge.
    /// </summary>
    BugHunterLevel2 = 1uL << 14,

    /// <summary>
    /// Undocumented and Private. User is pending deletion for being underage in DOB prompt.
    /// </summary>
    UnderageDeleted = 1uL << 15,

    // Verification Flags

    /// <summary>
    /// User is a verified bot. See <see href="https://support.discord.com/hc/articles/360040720412"/>.
    /// </summary>
    VerifiedBot = 1uL << 16,

    /// <summary>
    /// User has the 'Early Verified Developer' badge.
    /// </summary>
    VerifiedDeveloper = 1uL << 17,

    /// <summary>
    /// User has the 'Moderator Program Alumni' badge.
    /// </summary>
    CertifiedModerator = 1uL << 18,

    /// <summary>
    /// Undocumented. User is a bot with an interactions endpoint.
    /// </summary>
    BotHttpInteractions = 1uL << 19,

    /// <summary>
    /// Undocumented. User's account is disabled for spam.
    /// </summary>
    Spammer = 1uL << 20,

    /// <summary>
    /// Undocumented and Private. User's Nitro features are disabled.
    /// </summary>
    DisablePremium = 1uL << 21,

    /// <summary>
    /// User has the 'Active Developer' badge. See <see href="https://support-dev.discord.com/hc/articles/10113997751447"/>.
    /// </summary>
    ActiveDeveloper = 1uL << 22,

    /// <summary>
    /// Undocumented and Private. User's account has a high global rate limit.
    /// </summary>
    HighGlobalRateLimit = 1uL << 33,

    /// <summary>
    /// Undocumented and Private. User's account is deleted.
    /// </summary>
    Deleted = 1uL << 34,

    /// <summary>
    /// Undocumented and Private. User's account is disabled for suspicious activity.
    /// </summary>
    DisabledSuspiciousActivity = 1uL << 35,

    /// <summary>
    /// Undocumented and Private. User's account was manually deleted.
    /// </summary>
    SelfDeleted = 1uL << 36,

    /// <summary>
    /// Undocumented. User has a manually selected discriminator.
    /// </summary>
    PremiumDiscriminator = 1uL << 37,

    // Client Usage Flags
    
    /// <summary>
    /// Undocumented. User has used the desktop client.
    /// </summary>
    UsedDesktopClient = 1uL << 38,

    /// <summary>
    /// Undocumented. User has used the web client.
    /// </summary>
    UsedWebClient = 1uL << 39,

    /// <summary>
    /// Undocumented. User has used the mobile client.
    /// </summary>
    UsedMobileClient = 1uL << 40,

    /// <summary>
    /// Undocumented and Private. User's account is temporarily or permanently disabled.
    /// </summary>
    Disabled = 1uL << 41,

    /// <summary>
    /// Undocumented. User has a verified email.
    /// </summary>
    VerifiedEmail = 1uL << 43,

    /// <summary>
    /// Undocumented and Private. User is quarantined. See <see href="https://support.discord.com/hc/articles/6461420677527"/>.
    /// </summary>
    Quarantined = 1uL << 44,

    // Collaborator Flags
	
    /// <summary>
    /// Undocumented. User is a collaborator and has staff permissions.
    /// </summary>
    Collaborator = 1uL << 50,

    /// <summary>
    /// Undocumented. User is a restricted collaborator and has staff permissions.
    /// </summary>
    RestrictedCollaborator = 1uL << 51,
}
