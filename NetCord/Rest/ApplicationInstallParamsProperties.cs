using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationInstallParamsProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("scopes")]
    public IEnumerable<string>? Scopes { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("permissions")]
    public Permissions? Permissions { get; set; }
}
