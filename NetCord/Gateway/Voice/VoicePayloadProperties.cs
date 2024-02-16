using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace NetCord.Gateway.Voice;

internal class VoicePayloadProperties<T>(VoiceOpcode opcode, T d)
{
    [JsonPropertyName("op")]
    public VoiceOpcode Opcode { get; set; } = opcode;

    [JsonPropertyName("d")]
    public T D { get; set; } = d;

    public byte[] Serialize(JsonTypeInfo<VoicePayloadProperties<T>> jsonTypeInfo) => JsonSerializer.SerializeToUtf8Bytes(this, jsonTypeInfo);
}
