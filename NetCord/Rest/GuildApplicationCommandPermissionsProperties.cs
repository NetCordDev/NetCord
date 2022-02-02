using System.Text.Json.Serialization;

namespace NetCord;

public class GuildApplicationCommandPermissionsProperties
{
    [JsonPropertyName("id")]
    public DiscordId CommandId { get; }

    [JsonPropertyName("permissions")]
    public IEnumerable<ApplicationCommandPermissionProperties> Permissions { get; }

    public GuildApplicationCommandPermissionsProperties(DiscordId commandId, IEnumerable<ApplicationCommandPermissionProperties> permissions)
    {
        CommandId = commandId;
        Permissions = permissions;
    }
}