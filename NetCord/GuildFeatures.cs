namespace NetCord;

public class GuildFeatures
{
    public bool AnimatedIconAccess { get; }
    public bool BannerAccess { get; }
    public bool CommerceAccess { get; }
    public bool IsCommunity { get; }
    public bool IsDicoverable { get; }
    public bool IsFeaturable { get; }
    public bool InviteSplashAccess { get; }
    public bool MemberVerificationGateEnabled { get; }
    public bool NewsAccess { get; }
    public bool IsPartnered { get; }
    public bool IsPreviewEnabled { get; }
    public bool VanityUrlAccess { get; }
    public bool IsVerified { get; }
    public bool VipRegionsAccess { get; }
    public bool WelcomeScreenEnabled { get; }
    public bool TicketedEventsEnabled { get; }
    public bool MonetizationEnabled { get; }
    public bool CustomStickerSlots { get; }
    public bool ThreeDayThreadArchive { get; }
    public bool SevenDayThreadArchive { get; }
    public bool PrivateThreads { get; }

    internal GuildFeatures(string[] features)
    {
        AnimatedIconAccess = features.Contains("ANIMATED_ICON");
        BannerAccess = features.Contains("BANNER");
        CommerceAccess = features.Contains("COMMERCE");
        IsCommunity = features.Contains("COMMUNITY");
        IsDicoverable = features.Contains("DISCOVERABLE");
        IsFeaturable = features.Contains("FEATURABLE");
        InviteSplashAccess = features.Contains("INVITE_SPLASH");
        MemberVerificationGateEnabled = features.Contains("MEMBER_VERIFICATION_GATE_ENABLED");
        NewsAccess = features.Contains("NEWS");
        IsPartnered = features.Contains("PARTNERED");
        IsPreviewEnabled = features.Contains("PREVIEW_ENABLED");
        VanityUrlAccess = features.Contains("VANITY_URL");
        IsVerified = features.Contains("VERIFIED");
        VipRegionsAccess = features.Contains("VIP_REGIONS");
        WelcomeScreenEnabled = features.Contains("WELCOME_SCREEN_ENABLED");
        TicketedEventsEnabled = features.Contains("TICKETED_EVENTS_ENABLED");
        MonetizationEnabled = features.Contains("MONETIZATION_ENABLED");
        CustomStickerSlots = features.Contains("MORE_STICKERS");
    }
}