using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonPayload
{
    [JsonPropertyName("op")]
    public byte Opcode { get; init; }

    [JsonPropertyName("d")]
    public JsonElement? Data { get; init; }

    [JsonPropertyName("s")]
    public int? SequenceNumber { get; init; }

    [JsonPropertyName("t")]
    public string? Event { get; init; }
}