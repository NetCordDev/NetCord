using System.Text.Json.Serialization;

using NetCord.JsonModels;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
internal partial class JsonApplicationDeauthorizedWebhookEventData
{
    [JsonPropertyName("user")]
    public JsonUser User { get; set; }
}
