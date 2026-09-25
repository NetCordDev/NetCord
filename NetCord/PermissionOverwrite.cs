namespace NetCord;

/// <summary>
/// Represents the set of permission overwrites for a given user/role ID.
/// </summary>
public class PermissionOverwrite(JsonModels.JsonPermissionOverwrite jsonModel) : Entity, IJsonModel<JsonModels.JsonPermissionOverwrite>
{
    JsonModels.JsonPermissionOverwrite IJsonModel<JsonModels.JsonPermissionOverwrite>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the user/role affected by this overwrite.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// Specifies whether the overwrite applies to a user, or a role.
    /// </summary>
    public PermissionOverwriteType Type => jsonModel.Type;

    /// <summary>
    /// The set of permissions to grant the overwrite target.
    /// </summary>
    public Permissions Allowed => jsonModel.Allowed;

    /// <summary>
    /// The set of permissions to deny the overwrite target.
    /// </summary>
    public Permissions Denied => jsonModel.Denied;
}
