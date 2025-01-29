using NetCord.JsonModels;

namespace NetCord;

public class ApplicationIntegrationTypeConfiguration : IJsonModel<JsonApplicationIntegrationTypeConfiguration>
{
    public ApplicationIntegrationTypeConfiguration(JsonApplicationIntegrationTypeConfiguration jsonModel)
    {
        _jsonModel = jsonModel;

        var oAuth2InstallParams = jsonModel.OAuth2InstallParams;
        if (oAuth2InstallParams is not null)
            OAuth2InstallParams = new(oAuth2InstallParams);
    }

    private readonly JsonApplicationIntegrationTypeConfiguration _jsonModel;
    JsonApplicationIntegrationTypeConfiguration IJsonModel<JsonApplicationIntegrationTypeConfiguration>.JsonModel => _jsonModel;

    /// <summary>
    /// Install params for each installation context's default in-app authorization link.
    /// </summary>
    public ApplicationInstallParams? OAuth2InstallParams { get; }
}
