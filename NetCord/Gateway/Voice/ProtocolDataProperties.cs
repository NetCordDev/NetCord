using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class ProtocolDataProperties(string address, ushort port, string mode)
{
    [JsonPropertyName("address")]
    public string Address { get; set; } = address;

    [JsonPropertyName("port")]
    public ushort Port { get; set; } = port;

    [JsonPropertyName("mode")]
    public string Mode { get; set; } = mode;
}
