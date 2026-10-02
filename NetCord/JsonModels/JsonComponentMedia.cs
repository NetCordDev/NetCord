using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonComponentMedia
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("placeholder_version")]
    public int? PlaceholderVersion { get; set; }

    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("flags")]
    public ComponentMediaFlags? Flags { get; set; }

    [JsonPropertyName("attachment_id")]
    public ulong? AttachmentId { get; set; }
}
