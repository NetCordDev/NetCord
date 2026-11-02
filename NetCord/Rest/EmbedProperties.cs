using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <inheritdoc cref="Embed"/>
[GenerateMethodsForProperties]
public partial class EmbedProperties
{
    /// <summary>
    /// The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <see cref="Url"/>, has a 256 character limit.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <inheritdoc cref="Embed.Description"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// A link to an address of a webpage. When set, the <see cref="Title"/> becomes a clickable link, directing to the URL.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Displays time in a format similar to a message timestamp. Located next to the <see cref="Footer"/>, if one is set.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }

    /// <inheritdoc cref="Embed.Color"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("color")]
    public Color Color { get; set; }

    /// <inheritdoc cref="Embed.Footer"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("footer")]
    public EmbedFooterProperties? Footer { get; set; }

    /// <summary>
    /// The image included in the embed, displayed as a large-sized image located below the <see cref="Description"/> element.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("image")]
    public EmbedImageProperties? Image { get; set; }

    /// <inheritdoc cref="Embed.Thumbnail"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("thumbnail")]
    public EmbedImageProperties? Thumbnail { get; set; }

    /// <inheritdoc cref="Embed.Author"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("author")]
    public EmbedAuthorProperties? Author { get; set; }

    /// <summary>
    /// Allows the addition of multiple subtitles with additional content underneath them below the main <see cref="Title"/> and <see cref="Description"/> blocks, maximum of 25 per embed.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("fields")]
    public IEnumerable<EmbedFieldProperties>? Fields { get; set; }
}

/// <inheritdoc cref="EmbedFooter"/>
[GenerateMethodsForProperties]
public partial class EmbedFooterProperties
{

    /// <inheritdoc cref="EmbedFooter.Text"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Points to an image, which is displayed in a small circular format to the left of the <see cref="Text"/>.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
}

/// <inheritdoc cref="EmbedImage"/>
/// <param name="url"><inheritdoc cref="Url" path="/summary"/></param>
[GenerateMethodsForProperties]
public partial class EmbedImageProperties(string? url)
{
    /// <inheritdoc cref="EmbedImage.Url"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; } = url;

    public static implicit operator EmbedImageProperties(string? url) => new(url);
}

/// <inheritdoc cref="EmbedAuthor"/>
[GenerateMethodsForProperties]
public partial class EmbedAuthorProperties
{
    /// <inheritdoc cref="EmbedAuthor.Name"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <inheritdoc cref="EmbedAuthor.Url"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <inheritdoc cref="EmbedAuthor.IconUrl"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
}

/// <inheritdoc cref="EmbedField"/>
[GenerateMethodsForProperties]
public partial class EmbedFieldProperties
{
    /// <inheritdoc cref="EmbedField.Name"/>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc cref="EmbedField.Value"/>
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// When set alongside another field with <see cref="Inline"/> set, displays the fields side by side when supported.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("inline")]
    public bool Inline { get; set; }
}
