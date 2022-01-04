using System.Text.Json.Serialization;

using NetCord.JsonConverters;

namespace NetCord.JsonModels;

internal record JsonGuild : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("icon")]
    public string? Icon { get; init; }

    [JsonPropertyName("icon_hash")]
    public string? IconHash { get; init; }

    [JsonPropertyName("splash")]
    public string? SplashHash { get; init; }

    [JsonPropertyName("discovery_splash")]
    public string? DiscoverySplashHash { get; init; }

    [JsonPropertyName("owner")]
    public bool? IsOwner { get; init; }

    [JsonPropertyName("owner_id")]
    public DiscordId OwnerId { get; init; }

    [JsonPropertyName("permissions")]
    public string? Permissions { get; init; }

    [JsonPropertyName("afk_channel_id")]
    public DiscordId? AfkChannelId { get; init; }

    [JsonPropertyName("afk_timeout")]
    public int AfkTimeout { get; init; }

    [JsonPropertyName("widget_enabled")]
    public bool? WidgetEnabled { get; init; }

    [JsonPropertyName("widget_channel_id")]
    public DiscordId? WidgetChannelId { get; init; }

    [JsonPropertyName("verification_level")]
    public VerificationLevel VerificationLevel { get; init; }

    [JsonPropertyName("default_message_notifications")]
    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel { get; init; }

    [JsonPropertyName("explicit_content_filter")]
    public ContentFilter ContentFilter { get; init; }

    [JsonPropertyName("roles")]
    public JsonRole[] Roles { get; init; }

    [JsonPropertyName("emojis")]
    public JsonEmoji[] Emojis { get; init; }

    [JsonPropertyName("features")]
    public string[] Features { get; init; }

    [JsonPropertyName("mfa_level")]
    public MFALevel MFALevel { get; init; }

    [JsonPropertyName("application_id")]
    public DiscordId? ApplicationId { get; init; }

    [JsonPropertyName("system_channel_id")]
    public DiscordId? SystemChannelId { get; init; }

    [JsonPropertyName("system_channel_flags")]
    public SystemChannelFlags SystemChannelFlags { get; init; }

    [JsonPropertyName("rules_channel_id")]
    public DiscordId? RulesChannelId { get; init; }

    [JsonPropertyName("joined_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    [JsonPropertyName("large")]
    public bool? IsLarge { get; init; }

    [JsonPropertyName("unavailable")]
    public bool? IsUnavaible { get; init; }

    [JsonPropertyName("member_count")]
    public int? MemberCount { get; set; }

    [JsonPropertyName("voice_states")]
    public JsonVoiceState[] VoiceStates { get; init; }

    [JsonPropertyName("members")]
    public JsonGuildUser[] Users { get; init; }

    [JsonPropertyName("channels")]
    public JsonChannel[] Channels { get; set; }

    [JsonPropertyName("threads")]
    public JsonChannel[] ActiveThreads { get; init; }

    [JsonPropertyName("presences")]
    public JsonPresence[] Presences { get; init; }

    [JsonPropertyName("max_presences")]
    public int? MaxPresences { get; init; }

    [JsonPropertyName("max_members")]
    public int? MaxMembers { get; init; }

    [JsonPropertyName("vanity_url_code")]
    public string? VanityUrlCode { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("banner")]
    public string? BannerHash { get; init; }

    [JsonPropertyName("premium_tier")]
    public int PremiumTier { get; init; }

    [JsonPropertyName("premium_subscription_count")]
    public int? PremiumSubscriptionCount { get; init; }

    [JsonConverter(typeof(CultureInfoConverter))]
    [JsonPropertyName("preferred_locale")]
    public System.Globalization.CultureInfo PreferredLocale { get; init; }

    [JsonPropertyName("public_updates_channel_id")]
    public DiscordId? PublicUpdatesChannelId { get; init; }

    [JsonPropertyName("max_video_channel_users")]
    public int? MaxVideoChannelUsers { get; init; }

    [JsonPropertyName("approximate_member_count")]
    public int? ApproximateMemberCount { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public int? ApproximatePresenceCount { get; init; }

    [JsonPropertyName("welcome_screen")]
    public JsonWelcomeScreen? WelcomeScreen { get; init; }

    [JsonPropertyName("nsfw_level")]
    public NSFWLevel NSFWLevel { get; init; }

    [JsonPropertyName("stage_instances")]
    public JsonStageInstance[] StageInstances { get; init; }

    [JsonPropertyName("stickers")]
    public JsonSticker[] Stickers { get; init; }
}
