using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels;

public partial class JsonParty
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("size")]
    public int[]? Size { get; set; }

    [JsonSerializable(typeof(JsonParty))]
    public partial class JsonPartySerializerContext : JsonSerializerContext
    {
        public static JsonPartySerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
