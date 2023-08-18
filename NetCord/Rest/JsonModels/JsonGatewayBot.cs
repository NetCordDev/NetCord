using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public partial class JsonGatewayBot
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("shards")]
    public int ShardCount { get; set; }

    [JsonPropertyName("session_start_limit")]
    public JsonGatewaySessionStartLimit SessionStartLimit { get; set; }

    [JsonSerializable(typeof(JsonGatewayBot))]
    public partial class JsonGatewayBotSerializerContext : JsonSerializerContext
    {
        public static JsonGatewayBotSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
