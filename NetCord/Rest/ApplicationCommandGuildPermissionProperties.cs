using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class ApplicationCommandGuildPermissionProperties
{
    [JsonPropertyName("id")]
    public Snowflake Id { get; }

    [JsonPropertyName("type")]
    public ApplicationCommandGuildPermissionType Type { get; }

    [JsonPropertyName("permission")]
    public bool Permission { get; }

    public ApplicationCommandGuildPermissionProperties(Snowflake id, ApplicationCommandGuildPermissionType type, bool permission)
    {
        Id = id;
        Type = type;
        Permission = permission;
    }
}
