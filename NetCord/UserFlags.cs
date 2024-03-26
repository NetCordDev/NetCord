namespace NetCord;

[Flags]
public enum UserFlags : ulong
{
    /// <summary>
    /// User is a discord employee.
    /// </summary>
    Staff                      = (ulong)1 << 00,
    /// <summary>
    /// User is a discord partner.
    /// </summary>
    Partner                    = (ulong)1 << 01,

    /// <summary>
    /// User has the 'HypeSquad Events' badge.
    /// </summary>
    HypeSquad                  = (ulong)1 << 02, 
    /// <summary>
    /// User has the 'Bug Hunter' badge.
    /// </summary>
    BugHunterLevel1            = (ulong)1 << 03,
    /// <summary>
    /// Undocumented and Private. User has SMS recovery for 2FA enabled.
    /// </summary>
    MfaSms                     = (ulong)1 << 04,
    /// <summary>
    /// Undocumented and Private. User dismissed the Nitro promotion.
    /// </summary>
    PremiumPromoDismissed      = (ulong)1 << 05,

    // HypeSquad House Flags

    /// <summary>
    /// User is a House of Bravery Member.
    /// </summary>
    HypeSquadOnlineHouse1      = (ulong)1 << 06,
    /// <summary>
    /// User is a House of Brilliance Member.
    /// </summary>
    HypeSquadOnlineHouse2      = (ulong)1 << 07,
    /// <summary>
    /// User is a House of Balance Member.
    /// </summary>
    HypeSquadOnlineHouse3      = (ulong)1 << 08,

    /// <summary>
    /// User has the 'Early Supporter' badge.
    /// </summary>
    PremiumEarlySupporter      = (ulong)1 << 09,
    /// <summary>
    /// User is a team. See <see href="https://discord.com/developers/docs/topics/teams"/>.
    /// </summary>
    TeamPseudoUser             = (ulong)1 << 10,
    /// <summary>
    /// Undocumented and Private. User has a pending partner/verification application.
    /// </summary>
    InternalApplication        = (ulong)1 << 11,
    /// <summary>
    /// Undocumented. User is the SYSTEM account.
    /// </summary>
    System                     = (ulong)1 << 12,
    /// <summary>
    /// Undocumented and Private. User has unread messages from Discord.
    /// </summary>
    Has_Unread_Urgent_Messages = (ulong)1 << 13,
    /// <summary>
    /// User has the 'Golden Bug Hunter' badge.
    /// </summary>
    BugHunterLevel2            = (ulong)1 << 14,
    /// <summary>
    /// Undocumented and Private. User is pending deletion for being underage in DOB prompt.
    /// </summary>
    UnderageDeleted            = (ulong)1 << 15,

    // Verification Flags

    /// <summary>
    /// User is a verified bot. See <see href="https://support.discord.com/hc/articles/360040720412"/>.
    /// </summary>
    VerifiedBot                = (ulong)1 << 16,
    /// <summary>
    /// User has the 'Early Verified Developer' badge.
    /// </summary>
    VerifiedDeveloper          = (ulong)1 << 17,

    /// <summary>
    /// User has the 'Moderator Program Alumni' badge.
    /// </summary>
    CertifiedModerator         = (ulong)1 << 18,
    /// <summary>
    /// Undocumented. User is a bot with an interactions endpoint.
    /// </summary>
    BotHttpInteractions        = (ulong)1 << 19,
    /// <summary>
    /// Undocumented. User's account is disabled for spam.
    /// </summary>
    Spammer                    = (ulong)1 << 20,
    /// <summary>
    /// Undocumented and Private. User's Nitro features are disabled.
    /// </summary>
    DisablePremium             = (ulong)1 << 21, 
    /// <summary>
    /// User has the 'Active Developer' badge. See <see href="https://support-dev.discord.com/hc/articles/10113997751447"/>.
    /// </summary>
    ActiveDeveloper            = (ulong)1 << 22,
    /// <summary>
    /// Undocumented and Private. User's account has a high global rate limit.
    /// </summary>
    HighGlobalRateLimit        = (ulong)1 << 33,
    /// <summary>
    /// Undocumented and Private. User's account is deleted.
    /// </summary>
    Deleted                    = (ulong)1 << 34,
    /// <summary>
    /// Undocumented and Private. User's account is disabled for suspicious activity.
    /// </summary>
    DisabledSuspiciousActivity = (ulong)1 << 35,
    /// <summary>
    /// Undocumented and Private. User's account was manually deleted.
    /// </summary>
    SelfDeleted                = (ulong)1 << 36,
    /// <summary>
    /// Undocumented. User has a manually selected discriminator.
    /// </summary>
    PremiumDiscriminator       = (ulong)1 << 37,

    // Client Usage Flags
    
    /// <summary>
    /// Undocumented. User has used the desktop client.
    /// </summary>
    UsedDesktopClient          = (ulong)1 << 38,
    /// <summary>
    /// Undocumented. User has used the web client.
    /// </summary>
    UsedWebClient              = (ulong)1 << 39,
    /// <summary>
    /// Undocumented. User has used the mobile client.
    /// </summary>
    UsedMobileClient           = (ulong)1 << 40,

    /// <summary>
    /// Undocumented and Private. User's account is temporarily or permanently disabled.
    /// </summary>
    Disabled                   = (ulong)1 << 41,
    /// <summary>
    /// Undocumented. User has a verified email.
    /// </summary>
    VerifiedEmail              = (ulong)1 << 43,
    /// <summary>
    /// Undocumented and Private. User is quarantined. See <see href="https://support.discord.com/hc/articles/6461420677527"/>.
    /// </summary>
    Quarantined                = (ulong)1 << 44,

    // Collaborator Flags
	
    /// <summary>
    /// Undocumented. User is a collaborator and has staff permissions.
    /// </summary>
    Collaborator               = (ulong)1 << 50,
    /// <summary>
    /// Undocumented. User is a restricted collaborator and has staff permissions.
    /// </summary>
    RestrictedCollaborator     = (ulong)1 << 51,
}
