using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonApplicationCommandGuildPermission : JsonEntity
{
    [JsonPropertyName("type")]
    public ApplicationCommandGuildPermissionType Type { get; set; }

    [JsonPropertyName("permission")]
    public bool Permission { get; set; }

    [JsonSerializable(typeof(JsonApplicationCommandGuildPermission))]
    public partial class JsonApplicationCommandGuildPermissionSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationCommandGuildPermissionSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
