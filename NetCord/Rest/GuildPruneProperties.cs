using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GuildPruneProperties
{
    [JsonPropertyName("days")]
    public int Days { get; set; }

    [JsonPropertyName("compute_prune_count")]
    public bool ComputePruneCount { get; set; } = true;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("include_roles")]
    public IEnumerable<Snowflake>? Roles { get; set; }

    public GuildPruneProperties(int days)
    {
        Days = days;
    }
}
