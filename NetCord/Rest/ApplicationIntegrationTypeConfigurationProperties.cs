using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <inheritdoc cref="ApplicationIntegrationTypeConfiguration"/>
[GenerateMethodsForProperties]
public partial class ApplicationIntegrationTypeConfigurationProperties
{
    /// <inheritdoc cref="ApplicationIntegrationTypeConfiguration.OAuth2InstallParams"/>
    [JsonPropertyName("oauth2_install_params")]
    public ApplicationInstallParamsProperties? OAuth2InstallParams { get; set; }
}
