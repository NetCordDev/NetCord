using NetCord.JsonModels;

namespace NetCord;

public class ApplicationInstallParams(JsonApplicationInstallParams jsonModel) : IJsonModel<JsonApplicationInstallParams>
{
    JsonApplicationInstallParams IJsonModel<JsonApplicationInstallParams>.JsonModel => jsonModel;

    public IReadOnlyList<string> Scopes => jsonModel.Scopes;

    public Permissions Permissions => jsonModel.Permissions;
}
