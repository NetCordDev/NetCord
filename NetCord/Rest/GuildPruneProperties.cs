using System.Text.Json.Serialization;

namespace NetCord;

public class GuildPruneProperties
{
    [JsonPropertyName("days")]
    public int Days { get; set; }

    [JsonPropertyName("compute_prune_count")]
    public bool ComputePruneCount { get; set; } = true;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("include_roles")]
    public IEnumerable<DiscordId>? Roles { get; set; }
}