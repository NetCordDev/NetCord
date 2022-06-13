using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonEmbed
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }
    [JsonPropertyName("type")]
    public EmbedType? Type { get; init; }
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    [JsonPropertyName("url")]
    public string? Url { get; init; }
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; init; }
    [JsonPropertyName("color")]
    public Color? Color { get; init; }
    [JsonPropertyName("footer")]
    public JsonEmbedFooter? Footer { get; init; }
    [JsonPropertyName("image")]
    public JsonEmbedPartBase? Image { get; init; }
    [JsonPropertyName("thumbnail")]
    public JsonEmbedPartBase? Thumbnail { get; init; }
    [JsonPropertyName("video")]
    public JsonEmbedPartBase? Video { get; init; }
    [JsonPropertyName("provider")]
    public JsonEmbedProvider? Provider { get; init; }
    [JsonPropertyName("author")]
    public JsonEmbedAuthor? Author { get; init; }
    [JsonPropertyName("fields")]
    public JsonEmbedField[] Fields { get; init; }
}

public record JsonEmbedFooter
{
    [JsonPropertyName("text")]
    public string Text { get; init; }
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; init; }
    [JsonPropertyName("proxy_icon_url")]
    public string? ProxyIconUrl { get; init; }
}

public record JsonEmbedPartBase
{
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; init; }

    [JsonPropertyName("height")]
    public int? Height { get; init; }

    [JsonPropertyName("width")]
    public int? Width { get; init; }
}

public record JsonEmbedProvider
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}

public record JsonEmbedAuthor
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; init; }

    [JsonPropertyName("proxy_icon_url")]
    public string? ProxyIconUrl { get; init; }
}

public record JsonEmbedField
{
    [JsonPropertyName("name")]
    public string Title { get; init; }

    [JsonPropertyName("value")]
    public string Description { get; init; }

    [JsonPropertyName("inline")]
    public bool? Inline { get; init; }
}