namespace NetCord;

/// <summary>
/// Indicates the scope of an <see cref="ApplicationCommandPermission"/> object.
/// </summary>
public enum ApplicationCommandGuildPermissionType
{
    /// <summary>
    /// Indicates that a permission is overriden for a specific role.
    /// </summary>
    Role = 1,

    /// <summary>
    /// Indicates that a permission is overriden for a specific user.
    /// </summary>
    User = 2,

    /// <summary>
    /// Indicates that a permission is overriden for a specific channel.
    /// </summary>
    Channel = 3,
}
