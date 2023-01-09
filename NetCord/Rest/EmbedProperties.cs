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

    [JsonSerializable(typeof(EmbedProperties))]
    public partial class EmbedPropertiesSerializerContext : JsonSerializerContext
    {
        public static EmbedPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class EmbedFooterProperties
{
    /// <summary>
    /// Text of the footer.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Url of the footer icon.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }

    [JsonSerializable(typeof(EmbedFooterProperties))]
    public partial class EmbedFooterPropertiesSerializerContext : JsonSerializerContext
    {
        public static EmbedFooterPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class EmbedImageProperties
{
    /// <summary>
    /// Url of the image.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; }

    /// <summary>
    /// Url of the image.
    /// </summary>
    /// <param name="url"></param>
    public EmbedImageProperties(string? url)
    {
        Url = url;
    }

    public static implicit operator EmbedImageProperties(string? url) => new(url);

    public static implicit operator EmbedImageProperties(AttachmentProperties attachment) => FromAttachment(attachment.FileName);

    /// <summary>
    /// Creates new <see cref="EmbedImageProperties"/> based on <paramref name="attachmentFileName"/>.
    /// </summary>
    /// <param name="attachmentFileName">Attachment file name.</param>
    /// <returns></returns>
    public static EmbedImageProperties FromAttachment(string attachmentFileName) => new($"attachment://{attachmentFileName}");

    [JsonSerializable(typeof(EmbedImageProperties))]
    public partial class EmbedImagePropertiesSerializerContext : JsonSerializerContext
    {
        public static EmbedImagePropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class EmbedThumbnailProperties
{
    /// <summary>
    /// Url of the thumbnail.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">Url of the thumbnail.</param>
    public EmbedThumbnailProperties(string? url)
    {
        Url = url;
    }

    public static implicit operator EmbedThumbnailProperties(string? url) => new(url);

    [JsonSerializable(typeof(EmbedThumbnailProperties))]
    public partial class EmbedThumbnailPropertiesSerializerContext : JsonSerializerContext
    {
        public static EmbedThumbnailPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class EmbedAuthorProperties
{
    /// <summary>
    /// Name of the author.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Url of the author.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Url of the author icon.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }

    [JsonSerializable(typeof(EmbedAuthorProperties))]
    public partial class EmbedAuthorPropertiesSerializerContext : JsonSerializerContext
    {
        public static EmbedAuthorPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}

public partial class EmbedFieldProperties
{
    private const string Default = "\u00AD";

    /// <summary>
    /// Title of the field.
    /// </summary>
    [JsonPropertyName("name")]
    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                _title = Default;
            else
                _title = value;
        }
    }

    private string _title = Default;

    /// <summary>
    /// Description of the field.
    /// </summary>
    [JsonPropertyName("value")]
    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                _description = Default;
            else
                _description = value;
        }
    }

    private string _description = Default;

    /// <summary>
    /// Whether or not the field should display inline.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("inline")]
    public bool Inline { get; set; }

    [JsonSerializable(typeof(EmbedFieldProperties))]
    public partial class EmbedFieldPropertiesSerializerContext : JsonSerializerContext
    {
        public static EmbedFieldPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
