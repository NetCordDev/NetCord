using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels;

internal class JsonHello
{
    [JsonPropertyName("heartbeat_interval")]
    public double HeartbeatInterval { get; set; }
}
