using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonApplicationInstallParams
{
    [JsonPropertyName("scopes")]
    public required string[] Scopes { get; set; }

    [JsonPropertyName("permissions")]
    public required Permissions Permissions { get; set; }
}
