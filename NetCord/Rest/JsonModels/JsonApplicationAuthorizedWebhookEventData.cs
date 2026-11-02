using System.Text.Json.Serialization;

using NetCord.JsonModels;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
internal partial class JsonApplicationAuthorizedWebhookEventData
{
    [JsonPropertyName("integration_type")]
    public ApplicationIntegrationType? IntegrationType { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonPropertyName("scopes")]
    public string[] Scopes { get; set; }

    [JsonPropertyName("guild")]
    public JsonRestGuild? Guild { get; set; }
}
