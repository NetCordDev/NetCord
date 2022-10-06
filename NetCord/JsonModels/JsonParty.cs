using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonParty
{
    public string? Id { get; set; }

    [JsonPropertyName("size")]
    public int[]? Size { get; set; }

    [JsonSerializable(typeof(JsonParty))]
    public partial class JsonPartySerializerContext : JsonSerializerContext
    {
        public static JsonPartySerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
