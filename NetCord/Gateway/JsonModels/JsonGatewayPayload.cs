using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels;

internal class JsonGatewayPayload
{
    [JsonPropertyName("op")]
    public GatewayOpcode Opcode { get; set; }

    [JsonPropertyName("d")]
    public JsonElement? Data { get; set; }

    [JsonPropertyName("s")]
    public int? SequenceNumber { get; set; }

    [JsonPropertyName("t")]
    public string? Event { get; set; }
}
