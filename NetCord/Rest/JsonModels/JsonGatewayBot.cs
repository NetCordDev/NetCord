using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
public partial class JsonGatewayBot
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("shards")]
    public int ShardCount { get; set; }

    [JsonPropertyName("session_start_limit")]
    public JsonGatewaySessionStartLimit SessionStartLimit { get; set; }
}
