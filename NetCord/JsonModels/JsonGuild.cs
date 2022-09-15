using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonModels;

public record JsonGuild : JsonEntity
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
    public Snowflake OwnerId { get; init; }

    [JsonPropertyName("permissions")]
    public Permission? Permissions { get; init; }

    [JsonPropertyName("afk_channel_id")]
    public Snowflake? AfkChannelId { get; init; }

    [JsonPropertyName("afk_timeout")]
    public int AfkTimeout { get; init; }

    [JsonPropertyName("widget_enabled")]
    public bool? WidgetEnabled { get; init; }

    [JsonPropertyName("widget_channel_id")]
    public Snowflake? WidgetChannelId { get; init; }

    [JsonPropertyName("verification_level")]
    public VerificationLevel VerificationLevel { get; init; }

    [JsonPropertyName("default_message_notifications")]
    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel { get; init; }

    [JsonPropertyName("explicit_content_filter")]
    public ContentFilter ContentFilter { get; init; }

    [JsonConverter(typeof(JsonConverters.ArrayToDictionaryConverter<JsonGuildRole>))]
    [JsonPropertyName("roles")]
    public Dictionary<Snowflake, JsonGuildRole> Roles { get; init; }

    [JsonPropertyName("emojis")]
    public List<JsonEmoji> Emojis { get; init; }

    [JsonPropertyName("features")]
    public string[] Features { get; init; }

    [JsonPropertyName("mfa_level")]
    public MFALevel MFALevel { get; init; }

    [JsonPropertyName("application_id")]
    public Snowflake? ApplicationId { get; init; }

    [JsonPropertyName("system_channel_id")]
    public Snowflake? SystemChannelId { get; init; }

    [JsonPropertyName("system_channel_flags")]
    public SystemChannelFlags SystemChannelFlags { get; init; }

    [JsonPropertyName("rules_channel_id")]
    public Snowflake? RulesChannelId { get; init; }

    [JsonPropertyName("joined_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("large")]
    public bool IsLarge { get; init; }

    [JsonPropertyName("unavailable")]
    public bool IsUnavailable { get; init; }

    [JsonPropertyName("member_count")]
    public int UserCount { get; set; }

    [JsonConverter(typeof(JsonConverters.JsonVoiceStateArrayToDictionaryConverter))]
    [JsonPropertyName("voice_states")]
    public Dictionary<Snowflake, JsonVoiceState> VoiceStates { get; init; }

    [JsonConverter(typeof(JsonConverters.JsonGuildUserArrayToDictionaryConverter))]
    [JsonPropertyName("members")]
    public Dictionary<Snowflake, JsonGuildUser> Users { get; init; }

    [JsonConverter(typeof(JsonConverters.ArrayToDictionaryConverter<JsonChannel>))]
    [JsonPropertyName("channels")]
    public Dictionary<Snowflake, JsonChannel> Channels { get; set; }

    [JsonConverter(typeof(JsonConverters.ArrayToDictionaryConverter<JsonChannel>))]
    [JsonPropertyName("threads")]
    public Dictionary<Snowflake, JsonChannel> ActiveThreads { get; init; }

    [JsonConverter(typeof(JsonConverters.JsonPresenceArrayToDictionaryConverter))]
    [JsonPropertyName("presences")]
    public Dictionary<Snowflake, JsonPresence> Presences { get; init; }

    [JsonPropertyName("max_presences")]
    public int? MaxPresences { get; init; }

    [JsonPropertyName("max_members")]
    public int? MaxUsers { get; init; }

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

    [JsonPropertyName("preferred_locale")]
    public System.Globalization.CultureInfo PreferredLocale { get; init; }

    [JsonPropertyName("public_updates_channel_id")]
    public Snowflake? PublicUpdatesChannelId { get; init; }

    [JsonPropertyName("max_video_channel_users")]
    public int? MaxVideoChannelUsers { get; init; }

    [JsonPropertyName("approximate_member_count")]
    public int? ApproximateUserCount { get; set; }

    [JsonPropertyName("approximate_presence_count")]
    public int? ApproximatePresenceCount { get; init; }

    [JsonPropertyName("welcome_screen")]
    public JsonWelcomeScreen? WelcomeScreen { get; init; }

    [JsonPropertyName("nsfw_level")]
    public NSFWLevel NSFWLevel { get; init; }

    [JsonConverter(typeof(JsonConverters.ArrayToDictionaryConverter<JsonStageInstance>))]
    [JsonPropertyName("stage_instances")]
    public Dictionary<Snowflake, JsonStageInstance> StageInstances { get; init; }

    [JsonPropertyName("stickers")]
    public List<JsonSticker> Stickers { get; init; }

    [JsonConverter(typeof(JsonConverters.ArrayToDictionaryConverter<JsonGuildScheduledEvent>))]
    [JsonPropertyName("guild_scheduled_events")]
    public Dictionary<Snowflake, JsonGuildScheduledEvent> ScheduledEvents { get; init; }

    [JsonPropertyName("premium_progress_bar_enabled")]
    public bool PremiumPropressBarEnabled { get; init; }
}
