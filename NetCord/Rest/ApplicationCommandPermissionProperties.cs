using System.Text.Json.Serialization;

namespace NetCord;

public class ApplicationCommandPermissionProperties
{
    [JsonPropertyName("id")]
    public DiscordId Id { get; }

    [JsonPropertyName("type")]
    public ApplicationCommandPermissionType Type { get; }

    [JsonPropertyName("permission")]
    public bool Permission { get; }

    public ApplicationCommandPermissionProperties(DiscordId id, ApplicationCommandPermissionType type, bool permission)
    {
        Id = id;
        Type = type;
        Permission = permission;
    }
}