using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonChannel : JsonEntity
{
    [JsonPropertyName("type")]
    public ChannelType Type { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonPropertyName("permission_overwrites")]
    public JsonPermissionOverwrite[]? PermissionOverwrites { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    [JsonPropertyName("nsfw")]
    public bool? Nsfw { get; set; }

    [JsonPropertyName("last_message_id")]
    public ulong? LastMessageId { get; set; }

    [JsonPropertyName("bitrate")]
    public int? Bitrate { get; set; }

    [JsonPropertyName("user_limit")]
    public int? UserLimit { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public int? Slowmode { get; set; }

    [JsonPropertyName("recipients")]
    public JsonUser[]? Users { get; set; }

    [JsonPropertyName("icon")]
    public string? IconHash { get; set; }

    [JsonPropertyName("owner_id")]
    public ulong? OwnerId { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("parent_id")]
    public ulong? ParentId { get; set; }

    [JsonPropertyName("last_pin_timestamp")]
    public DateTimeOffset? LastPin { get; set; }

    [JsonPropertyName("rtc_region")]
    public string? RtcRegion { get; set; }

    [JsonPropertyName("video_quality_mode")]
    public VideoQualityMode? VideoQualityMode { get; set; }

    [JsonPropertyName("message_count")]
    public int? MessageCount { get; set; }

    [JsonPropertyName("member_count")]
    public int? UserCount { get; set; }

    [JsonPropertyName("thread_metadata")]
    public JsonGuildThreadMetadata? Metadata { get; set; }

    [JsonPropertyName("member")]
    public JsonThreadSelfUser? CurrentUser { get; set; }

    [JsonPropertyName("default_auto_archive_duration")]
    public int? DefaultAutoArchiveDuration { get; set; }

    [JsonPropertyName("permissions")]
    public Permission? Permissions { get; set; }

    [JsonPropertyName("flags")]
    public ChannelFlags? Flags { get; set; }

    [JsonPropertyName("total_message_sent")]
    public int? TotalMessageSent { get; set; }

    [JsonPropertyName("available_tags")]
    public JsonForumTag[]? AvailableTags { get; set; }

    [JsonPropertyName("applied_tags")]
    public ulong[]? AppliedTags { get; set; }

    [JsonPropertyName("default_reaction_emoji")]
    public JsonForumGuildChannelDefaultReaction? DefaultReactionEmoji { get; set; }

    [JsonPropertyName("default_thread_rate_limit_per_user")]
    public int? DefaultThreadSlowmode { get; set; }

    [JsonPropertyName("default_sort_order")]
    public SortOrderType? DefaultSortOrder { get; set; }

    [JsonPropertyName("default_forum_layout")]
    public ForumLayoutType? DefaultForumLayout { get; set; }

    [JsonPropertyName("message")]
    public JsonMessage? Message { get; set; }

    [JsonPropertyName("newly_created")]
    public bool? NewlyCreated { get; set; }

    [JsonSerializable(typeof(JsonChannel))]
    public partial class JsonChannelSerializerContext : JsonSerializerContext
    {
        public static JsonChannelSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(JsonChannel[]))]
    public partial class JsonChannelArraySerializerContext : JsonSerializerContext
    {
        public static JsonChannelArraySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
