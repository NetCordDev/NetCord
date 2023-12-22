using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class ProtocolDataProperties
{
    public ProtocolDataProperties(string address, ushort port, string mode)
    {
        Address = address;
        Port = port;
        Mode = mode;
    }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("port")]
    public ushort Port { get; set; }

    [JsonPropertyName("mode")]
    public string Mode { get; set; }
}
