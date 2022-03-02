using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

internal class PayloadProperties<TD>
{
    [JsonPropertyName("op")]
    public GatewayOpcode Opcode { get; }

    [JsonPropertyName("d")]
    public TD D { get; }

    public PayloadProperties(GatewayOpcode opcode, TD d)
    {
        Opcode = opcode;
        D = d;
    }

    public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(this);
}
