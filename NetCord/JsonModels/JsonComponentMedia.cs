using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonComponentMedia
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("loading_state")]
    public ComponentMediaLoadingState? LoadingState { get; set; }

    [JsonPropertyName("attachment_id")]
    public ulong? AttachmentId { get; set; }
}
