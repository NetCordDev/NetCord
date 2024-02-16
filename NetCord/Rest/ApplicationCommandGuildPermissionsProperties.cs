using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class ApplicationCommandGuildPermissionsProperties(IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions)
{
    [JsonPropertyName("permissions")]
    public IEnumerable<ApplicationCommandGuildPermissionProperties> NewPermissions { get; set; } = newPermissions;
}
