using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonApplicationCommandPermission : JsonEntity
{
    [JsonPropertyName("type")]
    public ApplicationCommandPermissionType Type { get; init; }

    [JsonPropertyName("permission")]
    public bool Permission { get; init; }
}