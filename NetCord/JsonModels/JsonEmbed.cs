using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEmbed
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("type")]
    public EmbedType? Type { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }

    [JsonPropertyName("color")]
    public Color? Color { get; set; }

    [JsonPropertyName("footer")]
    public JsonEmbedFooter? Footer { get; set; }

    [JsonPropertyName("image")]
    public JsonEmbedImage? Image { get; set; }

    [JsonPropertyName("thumbnail")]
    public JsonEmbedImage? Thumbnail { get; set; }

    [JsonPropertyName("video")]
    public JsonEmbedVideo? Video { get; set; }

    [JsonPropertyName("provider")]
    public JsonEmbedProvider? Provider { get; set; }

    [JsonPropertyName("author")]
    public JsonEmbedAuthor? Author { get; set; }

    [JsonPropertyName("fields")]
    public JsonEmbedField[]? Fields { get; set; }

    [JsonPropertyName("flags")]
    public EmbedFlags? Flags { get; set; }
}

public class JsonEmbedFooter
{
    [JsonPropertyName("text")]
    public required string Text { get; set; }

    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }

    [JsonPropertyName("proxy_icon_url")]
    public string? ProxyIconUrl { get; set; }
}

public class JsonEmbedImage
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("placeholder_version")]
    public int? PlaceholderVersion { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("flags")]
    public EmbedMediaFlags? Flags { get; set; }
}

public class JsonEmbedVideo
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("placeholder_version")]
    public int? PlaceholderVersion { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("flags")]
    public EmbedMediaFlags? Flags { get; set; }
}

public class JsonEmbedProvider
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class JsonEmbedAuthor
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }

    [JsonPropertyName("proxy_icon_url")]
    public string? ProxyIconUrl { get; set; }
}

public class JsonEmbedField
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("value")]
    public required string Value { get; set; }

    [JsonPropertyName("inline")]
    public bool? Inline { get; set; }
}
