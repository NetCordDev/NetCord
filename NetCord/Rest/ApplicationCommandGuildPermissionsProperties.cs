using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class ApplicationCommandGuildPermissionsProperties
{
    [JsonPropertyName("permissions")]
    public IEnumerable<ApplicationCommandGuildPermissionProperties> NewPermissions { get; set; }

    public ApplicationCommandGuildPermissionsProperties(IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions)
    {
        NewPermissions = newPermissions;
    }
}
