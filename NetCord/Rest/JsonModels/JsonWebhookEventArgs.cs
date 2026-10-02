using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonWebhookEventArgs
{
    [JsonPropertyName("version")]
    public required int Version { get; set; }

    [JsonPropertyName("application_id")]
    public required ulong ApplicationId { get; set; }

    [JsonPropertyName("type")]
    public required WebhookEventType Type { get; set; }

    [JsonPropertyName("event")]
    public JsonWebhookEventBody? Event { get; set; }
}
