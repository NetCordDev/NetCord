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
    public string Protocol { get; }

    [JsonPropertyName("data")]
    public ProtocolDataProperties Data { get; }
}

internal class ProtocolDataProperties
{
    public ProtocolDataProperties(string address, ushort port, string mode)
    {
        Address = address;
        Port = port;
        Mode = mode;
    }

    [JsonPropertyName("address")]
    public string Address { get; }

    [JsonPropertyName("port")]
    public ushort Port { get; }

    [JsonPropertyName("mode")]
    public string Mode { get; }
}
