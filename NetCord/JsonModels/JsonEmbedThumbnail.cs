using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEmbedThumbnail
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonSerializable(typeof(JsonEmbedThumbnail))]
    public partial class JsonEmbedThumbnailSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedThumbnailSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
