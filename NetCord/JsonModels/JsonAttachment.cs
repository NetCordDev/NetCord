using System.Net.Mime;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonAttachment : JsonEntity
{
    [JsonPropertyName("filename")]
    public string Filename { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonConverter(typeof(JsonConverters.ContentTypeConverter))]
    [JsonPropertyName("content_type")]
    public ContentType? ContentType { get; init; }

    [JsonPropertyName("size")]
    public int Size { get; init; }

    [JsonPropertyName("url")]
    public string Url { get; init; }

    [JsonPropertyName("proxy_url")]
    public string ProxyUrl { get; init; }

    [JsonPropertyName("height")]
    public int? Height { get; init; }

    [JsonPropertyName("width")]
    public int? Width { get; init; }

    [JsonPropertyName("ephemeral")]
    public bool Ephemeral { get; init; }
}
