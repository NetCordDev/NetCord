using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

internal class JsonApplicationAuthorizedWebhookEventData
{
    [JsonPropertyName("integration_id")]
    public IntegrationType? IntegrationType { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonPropertyName("scopes")]
    public string[] Scopes { get; set; }

    [JsonPropertyName("guild")]
    public JsonGuild? Guild { get; set; }
}
