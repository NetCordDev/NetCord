using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal partial class ApplicationCommandGuildPermissionsProperties
{
    [JsonPropertyName("permissions")]
    public IEnumerable<ApplicationCommandGuildPermissionProperties> NewPermissions { get; }

    public ApplicationCommandGuildPermissionsProperties(IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions)
    {
        NewPermissions = newPermissions;
    }

    [JsonSerializable(typeof(ApplicationCommandGuildPermissionsProperties))]
    public partial class ApplicationCommandGuildPermissionsPropertiesSerializerContext : JsonSerializerContext
    {
        public static ApplicationCommandGuildPermissionsPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
