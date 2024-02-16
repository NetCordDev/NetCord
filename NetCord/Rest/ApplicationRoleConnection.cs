namespace NetCord.Rest;

public class ApplicationRoleConnection(JsonModels.JsonApplicationRoleConnection jsonModel) : IJsonModel<JsonModels.JsonApplicationRoleConnection>
{
    JsonModels.JsonApplicationRoleConnection IJsonModel<JsonModels.JsonApplicationRoleConnection>.JsonModel => jsonModel;

    public string? PlatformName => jsonModel.PlatformName;
    public string? PlatformUsername => jsonModel.PlatformUsername;
    public IReadOnlyDictionary<string, string> Metadata => jsonModel.Metadata;
}
