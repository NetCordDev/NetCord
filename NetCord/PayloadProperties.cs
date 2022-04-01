using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

internal class PayloadProperties<T>
{
    [JsonPropertyName("op")]
    public GatewayOpcode Opcode { get; }

    [JsonPropertyName("d")]
    public T D { get; }

    public PayloadProperties(GatewayOpcode opcode, T d)
    {
        Opcode = opcode;
        D = d;
    }

    public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(this);
}
