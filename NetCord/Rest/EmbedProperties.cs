using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class EmbedProperties
{
    /// <summary>
    /// Title of the embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Description of the embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Url of the embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Timestamp of the embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Color of the embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("color")]
    public Color Color { get; set; }

    /// <summary>
    /// Footer of the embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("footer")]
    public EmbedFooterProperties? Footer { get; set; }

    /// <summary>
    /// Image of the embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("image")]
    public EmbedImageProperties? Image { get; set; }

    /// <summary>
    /// Thumbnail of the embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("thumbnail")]
    public EmbedThumbnailProperties? Thumbnail { get; set; }

    /// <summary>
    /// Author of the embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("author")]
    public EmbedAuthorProperties? Author { get; set; }

    /// <summary>
    /// Fields of the embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("fields")]
    public IEnumerable<EmbedFieldProperties>? Fields { get; set; }
}
