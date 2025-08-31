using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class ApplicationIntegrationTypeConfigurationProperties
{
    [JsonPropertyName("oauth2_install_params")]
    public ApplicationInstallParamsProperties? OAuth2InstallParams { get; set; }
}
