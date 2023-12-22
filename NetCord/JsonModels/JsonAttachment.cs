using System.Net.Mime;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAttachment : JsonEntity
{
    [JsonPropertyName("filename")]
    public string FileName { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonConverter(typeof(JsonConverters.ContentTypeConverter))]
    [JsonPropertyName("content_type")]
    public ContentType? ContentType { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("ephemeral")]
    public bool Ephemeral { get; set; }

    [JsonPropertyName("duration_secs")]
    public double? DurationSeconds { get; set; }

    [JsonPropertyName("waveform")]
    public byte[]? Waveform { get; set; }

    [JsonPropertyName("flags")]
    public AttachmentFlags Flags { get; set; }
}
