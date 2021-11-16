namespace NetCord
{
    public class SocketGuildFeatures
    {
        [Name("ANIMATED_ICON")]
        public bool AnimatedIconAcces { get; }
        [Name("BANNER")]
        public bool BannerAccess { get; }
        [Name("COMMERCE")]
        public bool CommerceAccess { get; }
        [Name("COMMUNITY")]
        public bool IsCommunity { get; }
        [Name("DISCOVERABLE")]
        public bool IsDicoverable { get; }
        [Name("FEATURABLE")]
        public bool IsFeaturable { get; }
        [Name("INVITE_SPLASH")]
        public bool InviteSplashAccess { get; }
        [Name("MEMBER_VERIFICATION_GATE_ENABLED")]
        public bool MemberVerificationGateEnabled { get; }
        [Name("NEWS")]
        public bool NewsAccess { get; }
        [Name("PARTNERED")]
        public bool IsPartnered { get; }
        [Name("PREVIEW_ENABLED")]
        public bool IsPreviewEnabled { get; }
        [Name("VANITY_URL")]
        public bool VanityUrlAccess { get; }
        [Name("VERIFIED")]
        public bool IsVerified { get; }
        [Name("VIP_REGIONS")]
        public bool VipRegionsAccess { get; }
        [Name("WELCOME_SCREEN_ENABLED")]
        public bool WelcomeScreenEnabled { get; }
        [Name("TICKETED_EVENTS_ENABLED")]
        public bool TicketedEventsEnabled { get; }
        [Name("MONETIZATION_ENABLED")]
        public bool MonetizationEnabled { get; }
        [Name("MORE_STICKERS")]
        public bool CustomStickerSlots { get; }
        [Name("THREE_DAY_THREAD_ARCHIVE")]
        public bool ThreeDayThreadArchive { get; }
        [Name("SEVEN_DAY_THREAD_ARCHIVE")]
        public bool SevenDayThreadArchive { get; }
        [Name("PRIVATE_THREADS")]
        public bool PrivateThreads { get; }

        internal SocketGuildFeatures(string[] features)
        {
            AnimatedIconAcces = features.Contains("ANIMATED_ICON");
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
}
