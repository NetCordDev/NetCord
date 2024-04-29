using NetCord.JsonModels;

namespace NetCord;

public class ApplicationInstallParams(JsonApplicationInstallParams jsonModel) : IJsonModel<JsonApplicationInstallParams>
{
    JsonApplicationInstallParams IJsonModel<JsonApplicationInstallParams>.JsonModel => jsonModel;

    /// <summary>
    /// Scopes to add the application to the server with.
    /// </summary>
    public IReadOnlyList<string> Scopes => jsonModel.Scopes;

    /// <summary>
    /// Permissions to request for the bot role.
    /// </summary>
    public Permissions Permissions => jsonModel.Permissions;
}
