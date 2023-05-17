using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEmbedFooter
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }

    [JsonPropertyName("proxy_icon_url")]
    public string? ProxyIconUrl { get; set; }

    [JsonSerializable(typeof(JsonEmbedFooter))]
    public partial class JsonEmbedFooterSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedFooterSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
