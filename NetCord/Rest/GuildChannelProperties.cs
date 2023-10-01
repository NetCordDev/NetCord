using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildChannelProperties
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public ChannelType Type { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("bitrate")]
    public int? Bitrate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("user_limit")]
    public int? UserLimit { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rate_limit_per_user")]
    public int? Slowmode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("permission_overwrites")]
    public IEnumerable<PermissionOverwriteProperties>? PermissionOverwrites { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("parent_id")]
    public ulong? ParentId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("nsfw")]
    public bool? Nsfw { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rtc_region")]
    public string? RtcRegion { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("video_quality_mode")]
    public VideoQualityMode? VideoQualityMode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_auto_archive_duration")]
    public int? DefaultAutoArchiveDuration { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_reaction_emoji")]
    public ForumGuildChannelDefaultReactionProperties? DefaultReactionEmoji { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("available_tags")]
    public IEnumerable<ForumTagProperties>? AvailableTags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_sort_order")]
    public SortOrderType? DefaultSortOrder { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_forum_layout")]
    public ForumLayoutType? DefaultForumLayout { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_thread_rate_limit_per_user")]
    public int? DefaultThreadSlowmode { get; set; }

    public GuildChannelProperties(string name, ChannelType type)
    {
        Name = name;
        Type = type;
    }

    [JsonSerializable(typeof(GuildChannelProperties))]
    public partial class GuildChannelPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildChannelPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
