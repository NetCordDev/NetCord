using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildApplicationCommandPermissionsProperties
{
    [JsonPropertyName("id")]
    public Snowflake CommandId { get; }

    [JsonPropertyName("permissions")]
    public IEnumerable<ApplicationCommandGuildPermissionProperties> Permissions { get; }

    public GuildApplicationCommandPermissionsProperties(Snowflake commandId, IEnumerable<ApplicationCommandGuildPermissionProperties> permissions)
    {
        CommandId = commandId;
        Permissions = permissions;
    }

    [JsonSerializable(typeof(GuildApplicationCommandPermissionsProperties))]
    public partial class GuildApplicationCommandPermissionsPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildApplicationCommandPermissionsPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
