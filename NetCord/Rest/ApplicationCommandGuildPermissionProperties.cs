using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationCommandGuildPermissionProperties
{
    /// <summary>
    /// Id of the role, user, or channel the permission is for. 'GuildId - 1' for all channels.
    /// </summary>
    [JsonPropertyName("id")]
    public ulong Id { get; }

    /// <summary>
    /// Type of the permission.
    /// </summary>
    [JsonPropertyName("type")]
    public ApplicationCommandGuildPermissionType Type { get; }

    /// <summary>
    /// <see langword="true"/> to allow, <see langword="false"/>, to disallow.
    /// </summary>
    [JsonPropertyName("permission")]
    public bool Permission { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">Id of the role, user, or channel the permission is for. 'GuildId - 1' for all channels.</param>
    /// <param name="type">Type of the permission.</param>
    /// <param name="permission"><see langword="true"/> to allow, <see langword="false"/>, to disallow.</param>
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
