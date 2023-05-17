using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels;

internal partial class JsonHello
{
    [JsonPropertyName("heartbeat_interval")]
    public double HeartbeatInterval { get; set; }

    [JsonSerializable(typeof(JsonHello))]
    public partial class JsonHelloSerializerContext : JsonSerializerContext
    {
        public static JsonHelloSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
