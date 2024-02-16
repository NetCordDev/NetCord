using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="id">Id of the role, user, or channel the permission is for. 'GuildId - 1' for all channels.</param>
/// <param name="type">Type of the permission.</param>
/// <param name="permission"><see langword="true"/> to allow, <see langword="false"/>, to disallow.</param>
public partial class ApplicationCommandGuildPermissionProperties(ulong id, ApplicationCommandGuildPermissionType type, bool permission)
{
    /// <summary>
    /// Id of the role, user, or channel the permission is for. 'GuildId - 1' for all channels.
    /// </summary>
    [JsonPropertyName("id")]
    public ulong Id { get; set; } = id;

    /// <summary>
    /// Type of the permission.
    /// </summary>
    [JsonPropertyName("type")]
    public ApplicationCommandGuildPermissionType Type { get; set; } = type;

    /// <summary>
    /// <see langword="true"/> to allow, <see langword="false"/>, to disallow.
    /// </summary>
    [JsonPropertyName("permission")]
    public bool Permission { get; set; } = permission;
}
