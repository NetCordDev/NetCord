using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;
internal class VoicePayloadProperties<T>
{
    [JsonPropertyName("op")]
    public VoiceOpcode Opcode { get; }

    [JsonPropertyName("d")]
    public T D { get; }

    public VoicePayloadProperties(VoiceOpcode opcode, T d)
    {
        Opcode = opcode;
        D = d;
    }

    public byte[] Serialize() => JsonSerializer.SerializeToUtf8Bytes(this);
}
