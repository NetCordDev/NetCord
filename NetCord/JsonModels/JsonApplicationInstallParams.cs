using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonApplicationInstallParams
{
    [JsonPropertyName("scopes")]
    public string[] Scopes { get; set; }

    [JsonPropertyName("permissions")]
    public Permissions Permissions { get; set; }
}
