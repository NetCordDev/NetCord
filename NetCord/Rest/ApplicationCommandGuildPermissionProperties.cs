using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationCommandGuildPermissionProperties
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

    [JsonSerializable(typeof(ApplicationCommandGuildPermissionProperties))]
    public partial class ApplicationCommandGuildPermissionPropertiesSerializerContext : JsonSerializerContext
    {
        public static ApplicationCommandGuildPermissionPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
