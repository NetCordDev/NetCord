using System.Text.Json.Serialization;

namespace NetCord;

public class ApplicationCommandOptions
{
    [JsonPropertyName("name")]
    public string? Name { get; }

    [JsonPropertyName("description")]
    public string? Description { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("options")]
    public IEnumerable<ApplicationCommandOptionProperties>? Options { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("default_permission")]
    public bool? DefaultPermission { get; set; }
}