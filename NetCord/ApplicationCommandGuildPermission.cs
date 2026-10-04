using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a permission override for a command in a guild.
/// </summary>
public class ApplicationCommandGuildPermission(JsonApplicationCommandGuildPermission jsonModel) : Entity(jsonModel)
{
    /// <summary>
    /// Indicates the scope of the permission override.
    /// </summary>
    public ApplicationCommandGuildPermissionType Type { get; } = jsonModel.Type;

    /// <summary>
    /// Indicates whether the override is intended to enable or disable a command.
    /// </summary>
    public bool Permission { get; } = jsonModel.Permission;
}
