using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class ProtocolProperties
{
    public ProtocolProperties(string protocol, ProtocolDataProperties data)
    {
        Protocol = protocol;
        Data = data;
    }

    [JsonPropertyName("protocol")]
    public string Protocol { get; set; }

    [JsonPropertyName("data")]
    public ProtocolDataProperties Data { get; set; }
}
