using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class GuildApplicationCommandPermissionsProperties
{
    [JsonPropertyName("id")]
    public Snowflake CommandId { get; }

    [JsonPropertyName("permissions")]
    public IEnumerable<ApplicationCommandPermissionProperties> Permissions { get; }

    public GuildApplicationCommandPermissionsProperties(Snowflake commandId, IEnumerable<ApplicationCommandPermissionProperties> permissions)
    {
        CommandId = commandId;
        Permissions = permissions;
    }
}