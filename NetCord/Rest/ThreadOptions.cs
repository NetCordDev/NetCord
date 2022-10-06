using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildThreadOptions
{
    internal GuildThreadOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("auto_archive_duration")]
    public int? AutoArchiveDuration { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("locked")]
    public bool? Locked { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("invitable")]
    public bool? Invitable { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rate_limit_per_user")]
    public int? Slowmode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("flags")]
    public ChannelFlags? Flags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("applied_tags")]
    public IEnumerable<Snowflake>? AppliedTags { get; set; }

    [JsonSerializable(typeof(GuildThreadOptions))]
    public partial class GuildThreadOptionsSerializerContext : JsonSerializerContext
    {
        public static GuildThreadOptionsSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
