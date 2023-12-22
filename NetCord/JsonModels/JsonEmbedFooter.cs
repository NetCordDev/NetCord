using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEmbedFooter
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }

    [JsonPropertyName("proxy_icon_url")]
    public string? ProxyIconUrl { get; set; }
}
