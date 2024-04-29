using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationIntegrationTypeConfigurationProperties
{
    [JsonPropertyName("oauth2_install_params")]
    public ApplicationInstallParamsProperties? OAuth2InstallParams { get; set; }
}
