using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonAccount : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonSerializable(typeof(JsonAccount))]
    public partial class JsonAccountSerializerContext : JsonSerializerContext
    {
        public static JsonAccountSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
