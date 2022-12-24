using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGatewayBot
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("shards")]
    public int Shards { get; set; }

    [JsonPropertyName("session_start_limit")]
    public JsonGatewaySessionStartLimit SessionStartLimit { get; set; }

    [JsonSerializable(typeof(JsonGatewayBot))]
    public partial class JsonGatewayBotSerializerContext : JsonSerializerContext
    {
        public static JsonGatewayBotSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
