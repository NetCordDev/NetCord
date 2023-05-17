using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEmbedVideo
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonSerializable(typeof(JsonEmbedVideo))]
    public partial class JsonEmbedVideoSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedVideoSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
