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
