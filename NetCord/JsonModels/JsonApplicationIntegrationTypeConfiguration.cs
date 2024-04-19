using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonApplicationIntegrationTypeConfiguration
{
    [JsonPropertyName("oauth2_install_params")]
    public JsonApplicationInstallParams? OAuth2InstallParams { get; set; }
}
