using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonApplicationCommandGuildPermission : JsonEntity
{
    [JsonPropertyName("type")]
    public ApplicationCommandGuildPermissionType Type { get; init; }

    [JsonPropertyName("permission")]
    public bool Permission { get; init; }
}