using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonWebhookEventArgs
{
    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("type")]
    public WebhookEventType Type { get; set; }

    [JsonPropertyName("event")]
    public JsonWebhookEventBody? Event { get; set; }
}
