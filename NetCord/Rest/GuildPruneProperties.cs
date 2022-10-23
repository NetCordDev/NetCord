using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildPruneProperties
{
    [JsonPropertyName("days")]
    public int Days { get; set; }

    [JsonPropertyName("compute_prune_count")]
    public bool ComputePruneCount { get; set; } = true;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("include_roles")]
    public IEnumerable<ulong>? Roles { get; set; }

    public GuildPruneProperties(int days)
    {
        Days = days;
    }

    [JsonSerializable(typeof(GuildPruneProperties))]
    public partial class GuildPrunePropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildPrunePropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
