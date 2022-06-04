using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Gateway;

internal class GatewayPayloadProperties<T>
{
    [JsonPropertyName("op")]
    public GatewayOpcode Opcode { get; }

    [JsonPropertyName("d")]
    public T D { get; }

    public GatewayPayloadProperties(GatewayOpcode opcode, T d)
    {
        Opcode = opcode;
        D = d;
    }

    public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(this);
}
