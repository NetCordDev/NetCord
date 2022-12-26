using NetCord.JsonModels;

namespace NetCord;

public class ApplicationInstallParams : IJsonModel<JsonApplicationInstallParams>
{
    JsonApplicationInstallParams IJsonModel<JsonApplicationInstallParams>.JsonModel => _jsonModel;
    private readonly JsonApplicationInstallParams _jsonModel;

    public ApplicationInstallParams(JsonApplicationInstallParams jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public IReadOnlyList<string> Scopes => _jsonModel.Scopes;

    public Permissions Permissions => _jsonModel.Permissions;
}
