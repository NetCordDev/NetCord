using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public partial class JsonGuild : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("icon_hash")]
    public string? IconHash { get; set; }

    [JsonPropertyName("splash")]
    public string? SplashHash { get; set; }

    [JsonPropertyName("discovery_splash")]
    public string? DiscoverySplashHash { get; set; }

    [JsonPropertyName("owner")]
    public bool? IsOwner { get; set; }

    [JsonPropertyName("owner_id")]
    public ulong OwnerId { get; set; }

    [JsonPropertyName("permissions")]
    public Permissions? Permissions { get; set; }

    [JsonPropertyName("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    [JsonPropertyName("afk_timeout")]
    public int AfkTimeout { get; set; }

    [JsonPropertyName("widget_enabled")]
    public bool? WidgetEnabled { get; set; }

    [JsonPropertyName("widget_channel_id")]
    public ulong? WidgetChannelId { get; set; }

    [JsonPropertyName("verification_level")]
    public VerificationLevel VerificationLevel { get; set; }

    [JsonPropertyName("default_message_notifications")]
    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel { get; set; }

    [JsonPropertyName("explicit_content_filter")]
    public ContentFilter ContentFilter { get; set; }

    [JsonConverter(typeof(JsonConverters.JsonGuildRoleArrayToDictionaryConverter))]
    [JsonPropertyName("roles")]
    public Dictionary<ulong, JsonGuildRole> Roles { get; set; }

    [JsonPropertyName("emojis")]
    public List<JsonEmoji> Emojis { get; set; }

    [JsonPropertyName("features")]
    public string[] Features { get; set; }

    [JsonPropertyName("mfa_level")]
    public MfaLevel MfaLevel { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    [JsonPropertyName("system_channel_flags")]
    public SystemChannelFlags SystemChannelFlags { get; set; }

    [JsonPropertyName("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }

    [JsonPropertyName("joined_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("large")]
    public bool IsLarge { get; set; }

    [JsonPropertyName("unavailable")]
    public bool IsUnavailable { get; set; }

    [JsonPropertyName("member_count")]
    public int UserCount { get; set; }

    [JsonConverter(typeof(JsonConverters.JsonVoiceStateArrayToDictionaryConverter))]
    [JsonPropertyName("voice_states")]
    public Dictionary<ulong, JsonVoiceState> VoiceStates { get; set; }

    [JsonConverter(typeof(JsonConverters.JsonGuildUserArrayToDictionaryConverter))]
    [JsonPropertyName("members")]
    public Dictionary<ulong, JsonGuildUser> Users { get; set; }

    [JsonConverter(typeof(JsonConverters.JsonChannelArrayToDictionaryConverter))]
    [JsonPropertyName("channels")]
    public Dictionary<ulong, JsonChannel> Channels { get; set; }

    [JsonConverter(typeof(JsonConverters.JsonChannelArrayToDictionaryConverter))]
    [JsonPropertyName("threads")]
    public Dictionary<ulong, JsonChannel> ActiveThreads { get; set; }

    [JsonConverter(typeof(JsonConverters.JsonPresenceArrayToDictionaryConverter))]
    [JsonPropertyName("presences")]
    public Dictionary<ulong, JsonPresence> Presences { get; set; }

    [JsonPropertyName("max_presences")]
    public int? MaxPresences { get; set; }

    [JsonPropertyName("max_members")]
    public int? MaxUsers { get; set; }

    [JsonPropertyName("vanity_url_code")]
    public string? VanityUrlCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("banner")]
    public string? BannerHash { get; set; }

    [JsonPropertyName("premium_tier")]
    public int PremiumTier { get; set; }

    [JsonPropertyName("premium_subscription_count")]
    public int? PremiumSubscriptionCount { get; set; }

    [JsonPropertyName("preferred_locale")]
    public System.Globalization.CultureInfo PreferredLocale { get; set; }

    [JsonPropertyName("public_updates_channel_id")]
    public ulong? PublicUpdatesChannelId { get; set; }

    [JsonPropertyName("max_video_channel_users")]
    public int? MaxVideoChannelUsers { get; set; }

    [JsonPropertyName("approximate_member_count")]
    public int? ApproximateUserCount { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public int? ApproximatePresenceCount { get; set; }

    [JsonPropertyName("welcome_screen")]
    public JsonWelcomeScreen? WelcomeScreen { get; set; }

    [JsonPropertyName("nsfw_level")]
    public NsfwLevel NsfwLevel { get; set; }

    [JsonConverter(typeof(JsonConverters.JsonStageInstanceArrayToDictionaryConverter))]
    [JsonPropertyName("stage_instances")]
    public Dictionary<ulong, JsonStageInstance> StageInstances { get; set; }

    [JsonPropertyName("stickers")]
    public List<JsonSticker> Stickers { get; set; }

    [JsonConverter(typeof(JsonConverters.JsonGuildScheduledEventArrayToDictionaryConverter))]
    [JsonPropertyName("guild_scheduled_events")]
    public Dictionary<ulong, JsonGuildScheduledEvent> ScheduledEvents { get; set; }

    [JsonPropertyName("premium_progress_bar_enabled")]
    public bool PremiumPropressBarEnabled { get; set; }

    [JsonSerializable(typeof(JsonGuild))]
    public partial class JsonGuildSerializerContext : JsonSerializerContext
    {
        public static JsonGuildSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(JsonGuild[]))]
    public partial class JsonGuildArraySerializerContext : JsonSerializerContext
    {
        public static JsonGuildArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
