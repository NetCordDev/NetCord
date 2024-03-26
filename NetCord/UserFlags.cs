namespace NetCord;

[Flags]
public enum UserFlags : long
{
    /// <summary> User is a discord employee. </summary>
    STAFF                        = (long) 1 << 00,
    /// <summary> User is a discord partner. </summary>
    PARTNER                      = (long) 1 << 01,

    /// <summary> User has the 'HypeSquad Events' badge. </summary>
    HYPESQUAD                    = (long) 1 << 02, 
    /// <summary> User has the 'Bug Hunter' badge. </summary>
    BUG_HUNTER_LEVEL_1           = (long) 1 << 03,
    /// <summary> Undocumented and Private. User has SMS recovery for 2FA enabled. </summary>
    MFA_SMS                      = (long) 1 << 04,
    /// <summary> Undocumented and Private. User dismissed the Nitro promotion. </summary>
    PREMIUM_PROMO_DISMISSED      = (long) 1 << 05,

    // HypeSquad House Flags

    /// <summary> User is a House of Bravery Member. </summary>
    HYPERSQUAD_ONLINE_HOUSE_1    = (long) 1 << 06,
    /// <summary> User is a House of Brilliance Member. </summary>
    HYPERSQUAD_ONLINE_HOUSE_2    = (long) 1 << 07,
    /// <summary> User is a House of Balance Member. </summary>
    HYPERSQUAD_ONLINE_HOUSE_3    = (long) 1 << 08,

    /// <summary> User has the 'Early Supporter' badge. </summary>
    PREMIUM_EARLY_SUPPORTER      = (long) 1 << 09,
    /// <summary> User is a team. See <see href="https://discord.com/developers/docs/topics/teams"/>. </summary>
    TEAM_PSEUDO_USER             = (long) 1 << 10,
    /// <summary> Undocumented and Private. User has a pending partner/verification application. </summary>
    INTERNAL_APPLICATION         = (long) 1 << 11,
    /// <summary> Undocumented. User is the SYSTEM account. </summary>
    SYSTEM                       = (long) 1 << 12,
    /// <summary> Undocumented and Private. User has unread messages from Discord. </summary>
    HAS_UNREAD_URGENT_MESSAGES   = (long) 1 << 13,
    /// <summary> User has the 'Golden Bug Hunter' badge. </summary>
    BUG_HUNTER_LEVEL_2           = (long) 1 << 14,
    /// <summary> Undocumented and Private. User is pending deletion for being underage in DOB prompt. </summary>
    UNDERAGE_DELETED             = (long) 1 << 15,

    // Verification Flags

    /// <summary> User is a verified bot. See <see href="https://support.discord.com/hc/articles/360040720412"/>. </summary>
    VERIFIED_BOT                 = (long) 1 << 16,
    /// <summary> User has the 'Early Verified Developer' badge. </summary>
    VERIFIED_DEVELOPER           = (long) 1 << 17,

    /// <summary> User has the 'Moderator Program Alumni' badge. </summary>
    CERTIFIED_MODERATOR          = (long) 1 << 18,
    /// <summary> Undocumented. User is a bot with an interactions endpoint. </summary>
    BOT_HTTP_INTERACTIONS        = (long) 1 << 19,
    /// <summary> Undocumented. User's account is disabled for spam. </summary>
    SPAMMER                      = (long) 1 << 20,
    /// <summary> Undocumented and Private. User's Nitro features are disabled. </summary>
    DISABLE_PREMIUM              = (long) 1 << 21, 
    /// <summary> User has the 'Active Developer' badge. See <see href="https://support-dev.discord.com/hc/articles/10113997751447"/>. </summary>
    ACTIVE_DEVELOPER             = (long) 1 << 22,
    /// <summary> Undocumented and Private. User's account has a high global rate limit. </summary>
    HIGH_GLOBAL_RATE_LIMIT       = (long) 1 << 33,
    /// <summary> Undocumented and Private. User's account is deleted. </summary>
    DELETED                      = (long) 1 << 34,
    /// <summary> Undocumented and Private. User's account is disabled for suspicious activity. </summary>
    DISABLED_SUSPICIOUS_ACTIVITY = (long) 1 << 35,
    /// <summary> Undocumented and Private. User's account was manually deleted. </summary>
    SELF_DELETED                 = (long) 1 << 36,
    /// <summary> Undocumented. User has a manually selected discriminator. </summary>
    PREMIUM_DISCRIMINATOR        = (long) 1 << 37,

    // Client Usage Flags
    
    /// <summary> Undocumented. User has used the desktop client. </summary>
    USED_DESKTOP_CLIENT          = (long) 1 << 38,
    /// <summary> Undocumented. User has used the web client. </summary>
    USED_WEB_CLIENT              = (long) 1 << 39,
    /// <summary> Undocumented. User has used the mobile client. </summary>
    USED_MOBILE_CLIENT           = (long) 1 << 40,

    /// <summary> Undocumented and Private. User's account is temporarily or permanently disabled. </summary>
    DISABLED                     = (long) 1 << 41,
    /// <summary> Undocumented. User has a verified email. </summary>
    VERIFIED_EMAIL               = (long) 1 << 43,
    /// <summary> Undocumented and Private. User is quarantined. See <see href="https://support.discord.com/hc/articles/6461420677527"/>. </summary>
    QUARANTINED                  = (long) 1 << 44,

    // Collaborator Flags
	
    /// <summary> Undocumented. User is a collaborator and has staff permissions. </summary>
    COLLABORATOR                 = (long) 1 << 50,
    /// <summary> Undocumented. User is a restricted collaborator and has staff permissions. </summary>
    RESTRICTED_COLLABORATOR      = (long) 1 << 51
}
