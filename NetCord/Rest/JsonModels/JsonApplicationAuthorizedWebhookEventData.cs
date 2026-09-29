using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

internal class JsonApplicationAuthorizedWebhookEventData
{
    [JsonPropertyName("integration_type")]
    public ApplicationIntegrationType? IntegrationType { get; set; }

    [JsonPropertyName("user")]
    public required JsonUser User { get; set; }

    [JsonPropertyName("scopes")]
    public required string[] Scopes { get; set; }

    [JsonPropertyName("guild")]
    public JsonRestGuild? Guild { get; set; }
}
