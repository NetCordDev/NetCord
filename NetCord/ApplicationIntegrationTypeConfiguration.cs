using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents additional configuration for a specific application installation context.
/// </summary>
public class ApplicationIntegrationTypeConfiguration(JsonApplicationIntegrationTypeConfiguration jsonModel) 
    : IJsonModel<JsonApplicationIntegrationTypeConfiguration>
{
    JsonApplicationIntegrationTypeConfiguration IJsonModel<JsonApplicationIntegrationTypeConfiguration>.JsonModel => jsonModel;

    /// <summary>
    /// Additional installation params to apply to the specific install context.
    /// </summary>
    public ApplicationInstallParams? OAuth2InstallParams { get; } = jsonModel.OAuth2InstallParams is { } oAuth2InstallParams ? new(oAuth2InstallParams) : null;
}
