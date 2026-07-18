using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents additional configuration for a specific app installation context.
/// </summary>
public class ApplicationIntegrationTypeConfiguration : IJsonModel<JsonApplicationIntegrationTypeConfiguration>
{
    private readonly JsonApplicationIntegrationTypeConfiguration _jsonModel;
    JsonApplicationIntegrationTypeConfiguration IJsonModel<JsonApplicationIntegrationTypeConfiguration>.JsonModel => _jsonModel;

    public ApplicationIntegrationTypeConfiguration(JsonApplicationIntegrationTypeConfiguration jsonModel)
    {
        _jsonModel = jsonModel;

        var oAuth2InstallParams = jsonModel.OAuth2InstallParams;
        if (oAuth2InstallParams is not null)
            OAuth2InstallParams = new(oAuth2InstallParams);
    }

    /// <summary>
    /// Additional installation params to apply to the specific install context.
    /// </summary>
    public ApplicationInstallParams? OAuth2InstallParams { get; }
}
