using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEmbedField
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }

    [JsonPropertyName("inline")]
    public bool Inline { get; set; }

    [JsonSerializable(typeof(JsonEmbedField))]
    public partial class JsonEmbedFieldSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedFieldSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
