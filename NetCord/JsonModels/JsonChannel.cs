using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

[JsonSourceGenerationOptions(IncludeFields = true)]
public record JsonChannel : JsonEntity
{
    [JsonPropertyName("type")]
    public ChannelType Type { get; init; }

    [JsonPropertyName("guild_id")]
    public Snowflake? GuildId { get; init; }

    [JsonPropertyName("position")]
    public int? Position { get; init; }

    [JsonPropertyName("permission_overwrites")]
    public JsonPermissionOverwrite[]? PermissionOverwrites { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("topic")]
    public string? Topic { get; init; }

    [JsonPropertyName("nsfw")]
    public bool IsNsfw { get; init; }

    [JsonPropertyName("last_message_id")]
    public Snowflake? LastMessageId { get; init; }

    [JsonPropertyName("bitrate")]
    public int? Bitrate { get; init; }

    [JsonPropertyName("user_limit")]
    public int? UserLimit { get; init; }

    [JsonPropertyName("rate_limit_per_user")]
    public int? Slowmode { get; init; }

    [JsonPropertyName("recipients")]
    public JsonUser[] Users { get; init; }

    [JsonPropertyName("icon")]
    public string IconHash { get; init; }

    [JsonPropertyName("owner_id")]
    public Snowflake? OwnerId { get; init; }

    [JsonPropertyName("application_id")]
    public Snowflake? ApplicationId { get; init; }

    [JsonPropertyName("parent_id")]
    public Snowflake? ParentId { get; init; }

    [JsonPropertyName("last_pin_timestamp")]
    public DateTimeOffset? LastPin { get; init; }

    [JsonPropertyName("rtc_region")]
    public string RtcRegion { get; init; }

    [JsonPropertyName("video_quality_mode")]
    public VideoQualityMode? VideoQualityMode { get; init; }

    [JsonPropertyName("message_count")]
    public int? MessageCount { get; init; }

    [JsonPropertyName("member_count")]
    public int? MemberCount { get; init; }

    [JsonPropertyName("thread_metadata")]
    public JsonThreadMetadata Metadata { get; init; }

    [JsonPropertyName("member")]
    public JsonThreadSelfUser? CurrentUser { get; init; }

    //[JsonPropertyName("default_auto_archive_duration")]
    //public int? DefaultAutoArchiveDuration { get; init; }

    [JsonPropertyName("permissions")]
    public string Permissions { get; init; }
}
