using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonPayload
{
    [JsonPropertyName("op")]
    public byte Opcode { get; set; }

    [JsonPropertyName("d")]
    public JsonElement? Data { get; set; }

    [JsonPropertyName("s")]
    public int? SequenceNumber { get; set; }

    [JsonPropertyName("t")]
    public string? Event { get; set; }

    [JsonSerializable(typeof(JsonPayload))]
    public partial class JsonPayloadSerializerContext : JsonSerializerContext
    {
        public static JsonPayloadSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
