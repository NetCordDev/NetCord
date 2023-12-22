using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonApplicationInstallParams
{
    [JsonPropertyName("scopes")]
    public string[] Scopes { get; set; }

    [JsonPropertyName("permissions")]
    public Permissions Permissions { get; set; }
}
