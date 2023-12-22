using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace NetCord.Gateway;

internal class GatewayPayloadProperties<T>
{
    [JsonPropertyName("op")]
    public GatewayOpcode Opcode { get; set; }

    [JsonPropertyName("d")]
    public T D { get; set; }

    public GatewayPayloadProperties(GatewayOpcode opcode, T d)
    {
        Opcode = opcode;
        D = d;
    }

    public byte[] Serialize(JsonTypeInfo<GatewayPayloadProperties<T>> jsonTypeInfo) => JsonSerializer.SerializeToUtf8Bytes(this, jsonTypeInfo);
}
