using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEmbedImage
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonSerializable(typeof(JsonEmbedImage))]
    public partial class JsonEmbedImageSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedImageSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
