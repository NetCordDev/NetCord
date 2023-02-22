using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonEmbed
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
    public JsonEmbedThumbnail? Thumbnail { get; set; }

    [JsonPropertyName("video")]
    public JsonEmbedVideo? Video { get; set; }

    [JsonPropertyName("provider")]
    public JsonEmbedProvider? Provider { get; set; }

    [JsonPropertyName("author")]
    public JsonEmbedAuthor? Author { get; set; }

    [JsonPropertyName("fields")]
    public JsonEmbedField[] Fields { get; set; }

    [JsonSerializable(typeof(JsonEmbed))]
    public partial class JsonEmbedSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

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

public partial class JsonEmbedImage
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonSerializable(typeof(JsonEmbedImage))]
    public partial class JsonEmbedImageSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedImageSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class JsonEmbedThumbnail
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonSerializable(typeof(JsonEmbedThumbnail))]
    public partial class JsonEmbedThumbnailSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedThumbnailSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class JsonEmbedVideo
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string? ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonSerializable(typeof(JsonEmbedVideo))]
    public partial class JsonEmbedVideoSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedVideoSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class JsonEmbedProvider
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonSerializable(typeof(JsonEmbedProvider))]
    public partial class JsonEmbedProviderSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedProviderSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

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

public partial class JsonEmbedField
{
    [JsonPropertyName("name")]
    public string Title { get; set; }

    [JsonPropertyName("value")]
    public string Description { get; set; }

    [JsonPropertyName("inline")]
    public bool? Inline { get; set; }

    [JsonSerializable(typeof(JsonEmbedField))]
    public partial class JsonEmbedFieldSerializerContext : JsonSerializerContext
    {
        public static JsonEmbedFieldSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
