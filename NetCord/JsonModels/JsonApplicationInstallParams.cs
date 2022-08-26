using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonApplicationInstallParams
{
    [JsonPropertyName("scopes")]
    public string[] Scopes { get; init; }

    [JsonPropertyName("permissions")]
    public Permission Permissions { get; init; }
}
