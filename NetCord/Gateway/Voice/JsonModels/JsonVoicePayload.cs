using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Gateway.Voice.JsonModels;

internal class JsonVoicePayload
{
    [JsonPropertyName("op")]
    public VoiceOpcode Opcode { get; set; }

    [JsonPropertyName("d")]
    public JsonElement? Data { get; set; }

    [JsonPropertyName("seq")]
    public int? SequenceNumber { get; set; }
}
