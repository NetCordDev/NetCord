using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildChannelProperties
{
    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("type")]
    public ChannelType? ChannelType { get; set; }

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
    public IEnumerable<ChannelPermissionOverwriteProperties>? PermissionOverwrites { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("parent_id")]
    public Snowflake? ParentId { get; set; }

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
    public DefaultReactionProperties? DefaultReactionEmoji { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("available_tags")]
    public IEnumerable<ForumTagProperties>? AvailableTags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_sort_order")]
    public SortOrderType? DefaultSortOrder { get; set; }

    public GuildChannelProperties(string name)
    {
        Name = name;
    }

    [JsonSerializable(typeof(GuildChannelProperties))]
    public partial class GuildChannelPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildChannelPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
