using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildApplicationCommandPermissionsProperties(ulong commandId, IEnumerable<ApplicationCommandGuildPermissionProperties> permissions)
{
    [JsonPropertyName("id")]
    public ulong CommandId { get; set; } = commandId;

    [JsonPropertyName("permissions")]
    public IEnumerable<ApplicationCommandGuildPermissionProperties> Permissions { get; set; } = permissions;
}
