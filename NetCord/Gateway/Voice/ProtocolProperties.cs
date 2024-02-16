using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class ProtocolProperties(string protocol, ProtocolDataProperties data)
{
    [JsonPropertyName("protocol")]
    public string Protocol { get; set; } = protocol;

    [JsonPropertyName("data")]
    public ProtocolDataProperties Data { get; set; } = data;
}
