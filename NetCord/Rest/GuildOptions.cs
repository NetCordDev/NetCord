using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents options for modifying a guild.
/// </summary>
/// <remarks>
/// Modifying a guild requires the <c>MANAGE_GUILD</c> permission. Adding or removing the
/// <c>COMMUNITY</c> guild feature requires the <c>ADMINISTRATOR</c> permission.
/// </remarks>
[GenerateMethodsForProperties]
public partial class GuildOptions
{
    internal GuildOptions()
    {
    }

    /// <summary>
    /// The new name of the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The new verification level of the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("verification_level")]
    public VerificationLevel? VerificationLevel { get; set; }

    /// <summary>
    /// The new default message notification level of the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_message_notifications")]
    public DefaultMessageNotificationLevel? DefaultMessageNotificationLevel { get; set; }

    /// <summary>
    /// The new explicit content filter level of the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("explicit_content_filter")]
    public ContentFilter? ContentFilter { get; set; }

    /// <summary>
    /// The ID of the new AFK channel.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("afk_channel_id")]
    public ulong? AfkChannelId { get; set; }

    /// <summary>
    /// The new AFK timeout, in seconds.
    /// </summary>
    /// <remarks>
    /// Discord accepts <c>60</c>, <c>300</c>, <c>900</c>, <c>1800</c>, or <c>3600</c>.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("afk_timeout")]
    public int? AfkTimeout { get; set; }

    /// <summary>
    /// The new guild icon.
    /// </summary>
    /// <remarks>
    /// Discord expects a 1024x1024 PNG, JPEG, or GIF image. Animated GIFs require the
    /// <c>ANIMATED_ICON</c> guild feature.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }

    /// <summary>
    /// The ID of the new guild owner.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("owner_id")]
    public ulong? OwnerId { get; set; }

    /// <summary>
    /// The new guild invite splash.
    /// </summary>
    /// <remarks>
    /// Discord expects a 16:9 PNG or JPEG image. The guild must have the
    /// <c>INVITE_SPLASH</c> feature.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("splash")]
    public ImageProperties? Splash { get; set; }

    /// <summary>
    /// The new guild discovery splash.
    /// </summary>
    /// <remarks>
    /// Discord expects a 16:9 PNG or JPEG image. The guild must have the
    /// <c>DISCOVERABLE</c> feature.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("discovery_splash")]
    public ImageProperties? DiscoverySplash { get; set; }

    /// <summary>
    /// The new guild banner.
    /// </summary>
    /// <remarks>
    /// Discord expects a 16:9 PNG, JPEG, or GIF image. The guild must have the
    /// <c>BANNER</c> feature. Animated GIFs additionally require the
    /// <c>ANIMATED_BANNER</c> feature.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("banner")]
    public ImageProperties? Banner { get; set; }

    /// <summary>
    /// The ID of the channel where guild notices such as welcome messages and boost events are posted.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("system_channel_id")]
    public ulong? SystemChannelId { get; set; }

    /// <summary>
    /// The new system channel flags.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("system_channel_flags")]
    public SystemChannelFlags? SystemChannelFlags { get; set; }

    /// <summary>
    /// The ID of the channel where a Community guild displays its rules or guidelines.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rules_channel_id")]
    public ulong? RulesChannelId { get; set; }

    /// <summary>
    /// The ID of the channel where administrators and moderators of a Community guild receive notices from Discord.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("public_updates_channel_id")]
    public ulong? PublicUpdatesChannelId { get; set; }

    /// <summary>
    /// The preferred locale of the Community guild.
    /// </summary>
    /// <remarks>
    /// The locale is used in server discovery and notices from Discord and defaults to <c>en-US</c>.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("preferred_locale")]
    public string? PreferredLocale { get; set; }

    /// <summary>
    /// The enabled guild features.
    /// </summary>
    /// <remarks>
    /// Discord currently documents <c>COMMUNITY</c>, <c>DISCOVERABLE</c>,
    /// <c>INVITES_DISABLED</c>, and <c>RAID_ALERTS_DISABLED</c> as mutable guild features.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("features")]
    public IEnumerable<string>? Features { get; set; }

    /// <summary>
    /// The new description of the guild.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Whether the guild's Server Boost progress bar should be enabled.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("premium_progress_bar_enabled")]
    public bool? PremiumProgressBarEnabled { get; set; }

    /// <summary>
    /// The ID of the channel where administrators and moderators of a Community guild receive safety alerts from Discord.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("safety_alerts_channel_id")]
    public ulong? SafetyAlertsChannelId { get; set; }
}
