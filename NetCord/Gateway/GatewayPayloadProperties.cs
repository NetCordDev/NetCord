using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace NetCord.Gateway;

internal class GatewayPayloadProperties<T>(GatewayOpcode opcode, T d)
{
    [JsonPropertyName("op")]
    public GatewayOpcode Opcode { get; set; } = opcode;

    [JsonPropertyName("d")]
    public T D { get; set; } = d;

    public byte[] Serialize(JsonTypeInfo<GatewayPayloadProperties<T>> jsonTypeInfo) => JsonSerializer.SerializeToUtf8Bytes(this, jsonTypeInfo);
}
