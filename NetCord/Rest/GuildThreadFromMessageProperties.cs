using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildThreadFromMessageProperties
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("auto_archive_duration")]
    public int? AutoArchiveDuration { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rate_limit_per_user")]
    public int? Slowmode { get; set; }

    public GuildThreadFromMessageProperties(string name)
    {
        Name = name;
    }
}
