using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class ThreadWithMessageProperties
{
    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("auto_archive_duration")]
    public int? AutoArchiveDuration { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("rate_limit_per_user")]
    public int? Slowmode { get; set; }

    public ThreadWithMessageProperties(string name)
    {
        Name = name;
    }
}
