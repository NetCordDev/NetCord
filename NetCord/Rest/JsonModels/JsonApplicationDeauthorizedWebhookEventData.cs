using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

internal class JsonApplicationDeauthorizedWebhookEventData
{
    [JsonPropertyName("user")]
    public required JsonUser User { get; set; }
}
