using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationCommandGuildPermissionProperties
{
    [JsonPropertyName("id")]
    public ulong Id { get; }

    [JsonPropertyName("type")]
    public ApplicationCommandGuildPermissionType Type { get; }

    [JsonPropertyName("permission")]
    public bool Permission { get; }

    public ApplicationCommandGuildPermissionProperties(ulong id, ApplicationCommandGuildPermissionType type, bool permission)
    {
        Id = id;
        Type = type;
        Permission = permission;
    }

    [JsonSerializable(typeof(ApplicationCommandGuildPermissionProperties))]
    public partial class ApplicationCommandGuildPermissionPropertiesSerializerContext : JsonSerializerContext
    {
        public static ApplicationCommandGuildPermissionPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
