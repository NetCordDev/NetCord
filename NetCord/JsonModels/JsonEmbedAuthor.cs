using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEmbedAuthor
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }

    [JsonPropertyName("proxy_icon_url")]
    public string? ProxyIconUrl { get; set; }

    [JsonSerializable(typeof(JsonEmbedAuthor))]
    public partial class JsonEmbedAuthorSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedAuthorSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
