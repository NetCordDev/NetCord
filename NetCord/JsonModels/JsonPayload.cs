using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal class JsonPayload
{
    [JsonPropertyName("op")]
    public GatewayOpcode Opcode { get; init; }

    [JsonPropertyName("d")]
    public JsonElement? Data { get; init; }

    [JsonPropertyName("s")]
    public int? SequenceNumber { get; init; }

    [JsonPropertyName("t")]
    public string? Event { get; init; }
}
