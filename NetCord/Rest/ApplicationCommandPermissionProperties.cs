using System.Text.Json.Serialization;

namespace NetCord;

public class ApplicationCommandPermissionProperties
{
    [JsonPropertyName("id")]
    public Snowflake Id { get; }

    [JsonPropertyName("type")]
    public ApplicationCommandPermissionType Type { get; }

    [JsonPropertyName("permission")]
    public bool Permission { get; }

    public ApplicationCommandPermissionProperties(Snowflake id, ApplicationCommandPermissionType type, bool permission)
    {
        Id = id;
        Type = type;
        Permission = permission;
    }
}