using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonWebhookEventBody
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}
