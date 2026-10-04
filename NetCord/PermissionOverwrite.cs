namespace NetCord;

/// <summary>
/// Represents the set of permission overwrites for a given user/role ID.
/// </summary>
public class PermissionOverwrite(JsonModels.JsonPermissionOverwrite jsonModel) : Entity
{
    /// <summary>
    /// The ID of the user/role affected by this overwrite.
    /// </summary>
    public override ulong Id { get; } = jsonModel.Id;

    /// <summary>
    /// Specifies whether the overwrite applies to a user, or a role.
    /// </summary>
    public PermissionOverwriteType Type { get; } = jsonModel.Type;

    /// <summary>
    /// The set of permissions to grant the overwrite target.
    /// </summary>
    public Permissions Allowed { get; } = jsonModel.Allowed;

    /// <summary>
    /// The set of permissions to deny the overwrite target.
    /// </summary>
    public Permissions Denied { get; } = jsonModel.Denied;
}
