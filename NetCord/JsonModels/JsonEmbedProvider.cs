using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEmbedProvider
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonSerializable(typeof(JsonEmbedProvider))]
    public partial class JsonEmbedProviderSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedProviderSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
