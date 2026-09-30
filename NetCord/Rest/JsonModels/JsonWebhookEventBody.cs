using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonWebhookEventBody
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}
