using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice;

internal class VoiceMessageProperties<T>(VoiceOpcode opcode, T d)
{
    [JsonPropertyName("op")]
    public VoiceOpcode Opcode { get; set; } = opcode;

    [JsonPropertyName("d")]
    public T D { get; set; } = d;
}
