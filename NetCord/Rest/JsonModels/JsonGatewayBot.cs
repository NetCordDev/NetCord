using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonGatewayBot
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("shards")]
    public required int ShardCount { get; set; }

    [JsonPropertyName("session_start_limit")]
    public required JsonGatewaySessionStartLimit SessionStartLimit { get; set; }
}
