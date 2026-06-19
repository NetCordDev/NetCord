using System.Text.Json.Serialization;

namespace NetCord.Gateway;

internal class GatewayPayloadProperties<T>(GatewayOpcode opcode, T d)
{
    [JsonPropertyName("op")]
    public GatewayOpcode Opcode { get; set; } = opcode;

    [JsonPropertyName("d")]
    public T D { get; set; } = d;
}
