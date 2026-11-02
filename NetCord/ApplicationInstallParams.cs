using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents the configuration state for an application install.
/// </summary>
public class ApplicationInstallParams(JsonApplicationInstallParams jsonModel)
{
    /// <summary>
    /// The scopes to use when installing the application to the guild.
    /// </summary>
    public IReadOnlyList<string> Scopes { get; } = jsonModel.Scopes;

    /// <summary>
    /// The permissions to request for the application's bot role.
    /// </summary>
    public Permissions Permissions { get; } = jsonModel.Permissions;
}
