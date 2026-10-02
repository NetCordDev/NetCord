using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonRestGuild : JsonPartialGuild
{
    [JsonPropertyName("icon_hash")]
    public string? IconHashTemplate { get; set; }

    [JsonPropertyName("discovery_splash")]
    public string? DiscoverySplashHash { get; set; }

    [JsonPropertyName("owner")]
    public bool? IsOwner { get; set; }

    [JsonPropertyName("owner_id")]
    public required ulong OwnerId { get; set; }

    [JsonPropertyName("permissions")]
    public Permissions? Permissions { get; set; }

    [JsonPropertyName("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    [JsonPropertyName("afk_timeout")]
    public required int AfkTimeout { get; set; }

    [JsonPropertyName("widget_enabled")]
    public bool? WidgetEnabled { get; set; }

    [JsonPropertyName("widget_channel_id")]
    public ulong? WidgetChannelId { get; set; }

    [JsonPropertyName("default_message_notifications")]
    public required DefaultMessageNotificationLevel DefaultMessageNotificationLevel { get; set; }

    [JsonPropertyName("explicit_content_filter")]
    public required ContentFilter ContentFilter { get; set; }

    [JsonPropertyName("roles")]
    public required JsonRole[] Roles { get; set; }

    [JsonPropertyName("emojis")]
    public required JsonEmoji[] Emojis { get; set; }

    [JsonPropertyName("mfa_level")]
    public required MfaLevel MfaLevel { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    [JsonPropertyName("system_channel_flags")]
    public required SystemChannelFlags SystemChannelFlags { get; set; }

    [JsonPropertyName("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }

    [JsonPropertyName("max_presences")]
    public int? MaxPresences { get; set; }

    [JsonPropertyName("max_members")]
    public int? MaxUsers { get; set; }

    [JsonPropertyName("premium_tier")]
    public required int PremiumTier { get; set; }

    [JsonPropertyName("premium_subscription_count")]
    public int? PremiumSubscriptionCount { get; set; }

    [JsonPropertyName("preferred_locale")]
    public required string PreferredLocale { get; set; }

    [JsonPropertyName("public_updates_channel_id")]
    public ulong? PublicUpdatesChannelId { get; set; }

    [JsonPropertyName("max_video_channel_users")]
    public int? MaxVideoChannelUsers { get; set; }

    [JsonPropertyName("max_stage_video_channel_users")]
    public int? MaxStageVideoChannelUsers { get; set; }

    [JsonPropertyName("approximate_member_count")]
    public int? ApproximateUserCount { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public int? ApproximatePresenceCount { get; set; }

    [JsonPropertyName("nsfw_level")]
    public required NsfwLevel NsfwLevel { get; set; }

    [JsonPropertyName("stickers")]
    public required JsonSticker[] Stickers { get; set; }

    [JsonPropertyName("premium_progress_bar_enabled")]
    public required bool PremiumProgressBarEnabled { get; set; }

    [JsonPropertyName("safety_alerts_channel_id")]
    public ulong? SafetyAlertsChannelId { get; set; }

    [JsonPropertyName("incidents_data")]
    public JsonGuildIncidentsData? IncidentsData { get; set; }
}
