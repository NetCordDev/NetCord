using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildPruneProperties(int days)
{
    [JsonPropertyName("days")]
    public int Days { get; set; } = days;

    [JsonPropertyName("compute_prune_count")]
    public bool ComputePruneCount { get; set; } = true;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("include_roles")]
    public IEnumerable<ulong>? Roles { get; set; }
}
