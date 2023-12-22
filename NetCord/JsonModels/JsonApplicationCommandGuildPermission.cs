using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonApplicationCommandGuildPermission : JsonEntity
{
    [JsonPropertyName("type")]
    public ApplicationCommandGuildPermissionType Type { get; set; }

    [JsonPropertyName("permission")]
    public bool Permission { get; set; }
}
