using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAttachment : JsonEntity
{
    [JsonPropertyName("filename")]
    public string FileName { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder;

    [JsonPropertyName("placeholder_version")]
    public int? PlaceholderVersion { get; set; }

    [JsonPropertyName("ephemeral")]
    public bool Ephemeral { get; set; }

    [JsonPropertyName("duration_secs")]
    public double? DurationSeconds { get; set; }

    [JsonPropertyName("waveform")]
    public byte[]? Waveform { get; set; }

    [JsonPropertyName("flags")]
    public AttachmentFlags Flags { get; set; }

    [JsonPropertyName("clip_participants")]
    public JsonUser[]? ClipParticipants { get; set; }

    [JsonPropertyName("clip_created_at")]
    public DateTimeOffset? ClipCreatedAt { get; set; }

    [JsonPropertyName("application")]
    public JsonApplication? Application { get; set; }
}
