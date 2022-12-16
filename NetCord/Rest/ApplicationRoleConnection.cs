namespace NetCord.Rest;

public class ApplicationRoleConnection : IJsonModel<JsonModels.JsonApplicationRoleConnection>
{
    JsonModels.JsonApplicationRoleConnection IJsonModel<JsonModels.JsonApplicationRoleConnection>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplicationRoleConnection _jsonModel;

    public ApplicationRoleConnection(JsonModels.JsonApplicationRoleConnection jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public string? PlatformName => _jsonModel.PlatformName;
    public string? PlatformUsername => _jsonModel.PlatformUsername;
    public IReadOnlyDictionary<string, string> Metadata => _jsonModel.Metadata;
}
