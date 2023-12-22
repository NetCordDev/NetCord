using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace NetCord.Gateway.Voice;

internal class VoicePayloadProperties<T>
{
    [JsonPropertyName("op")]
    public VoiceOpcode Opcode { get; set; }

    [JsonPropertyName("d")]
    public T D { get; set; }

    public VoicePayloadProperties(VoiceOpcode opcode, T d)
    {
        Opcode = opcode;
        D = d;
    }

    public byte[] Serialize(JsonTypeInfo<VoicePayloadProperties<T>> jsonTypeInfo) => JsonSerializer.SerializeToUtf8Bytes(this, jsonTypeInfo);
}
