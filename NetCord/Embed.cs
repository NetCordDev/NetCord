using System.Text.Json.Serialization;

namespace NetCord;

/// <summary>
/// Displays embedded content such as an image or URL, alongside a title and various other fields. You can only have up to 10 embeds per message, and the total text of all embeds must be less than or equal to 6000 characters.
/// </summary>
public class Embed(JsonModels.JsonEmbed jsonModel)
{
    /// <summary>
    /// The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <see cref="Url"/>, has a 256 character limit.
    /// </summary>
    public string? Title { get; } = jsonModel.Title;

    /// <summary>
    /// Information about the type of the embed.
    /// </summary>
    public EmbedType? Type { get; } = jsonModel.Type;

    /// <summary>
    /// The part of the embed where the main text is contained, limited to 4096 characters.
    /// </summary>
    public string? Description { get; } = jsonModel.Description;

    /// <summary>
    /// A link to an address of a webpage. When set, the <see cref="Title"/> becomes a clickable link, directing to the URL.
    /// </summary>
    public string? Url { get; } = jsonModel.Url;

    /// <summary>
    /// Displays time in a format similar to a message timestamp. Located next to the <see cref="Footer"/>.
    /// </summary>
    public DateTimeOffset? Timestamp { get; } = jsonModel.Timestamp;

    /// <summary>
    /// The color of the embed's border in an RGB format.
    /// </summary>
    public Color? Color { get; } = jsonModel.Color;

    /// <summary>
    /// The text at the bottom of the embed, limited to 2048 characters.
    /// </summary>
    public EmbedFooter? Footer { get; } = jsonModel.Footer is { } footer ? new(footer) : null;

    /// <summary>
    /// The image included in the embed, displayed as a large-sized image located below the <see cref="Description"/> element.
    /// </summary>
    public EmbedImage? Image { get; } = jsonModel.Image is { } image ? new(image) : null;

    /// <summary>
    /// The thumbnail of the embed, displayed as a medium-sized image in the top right corner of the embed.
    /// </summary>
    public EmbedImage? Thumbnail { get; } = jsonModel.Thumbnail is { } thumbnail ? new(thumbnail) : null;

    /// <summary>
    /// The video included and displayed in the embed.
    /// </summary>
    public EmbedVideo? Video { get; } = jsonModel.Video is { } video ? new(video) : null;

    /// <summary>
    /// The provider of the embed content (YouTube, Twitter/X, etc), automatically generated from links in content.
    /// </summary>
    public EmbedProvider? Provider { get; } = jsonModel.Provider is { } provider ? new(provider) : null;

    /// <summary>
    /// Contains the author block, always located at the top of the embed.
    /// </summary>
    public EmbedAuthor? Author { get; } = jsonModel.Author is { } author ? new(author) : null;

    /// <summary>
    /// Allows the addition of multiple subtitles with additional content underneath them below the main <see cref="Title"/> and <see cref="Description"/> blocks, maximum of 25 per embed.
    /// </summary>
    public IReadOnlyList<EmbedField>? Fields { get; } = jsonModel.Fields?.Select(f => new EmbedField(f)).ToArray();

    /// <summary>
    /// Embed flags combined as a bitfield.
    /// </summary>
    public EmbedFlags? Flags { get; } = jsonModel.Flags;
}

[JsonConverter(typeof(JsonConverters.SafeStringEnumConverter<EmbedType>))]
public enum EmbedType : sbyte
{
    [JsonPropertyName("rich")]
    Rich,

    [JsonPropertyName("image")]
    Image,

    [JsonPropertyName("video")]
    Video,

    [JsonPropertyName("gifv")]
    Gifv,

    [JsonPropertyName("article")]
    Article,

    [JsonPropertyName("link")]
    Link,

    [JsonPropertyName("poll_result")]
    PollResult,
}

/// <summary>
/// Contains information used to render the footer block of an embed.
/// </summary>
public class EmbedFooter(JsonModels.JsonEmbedFooter jsonModel)
{
    /// <summary>
    /// The text displayed in the footer, to the right of the icon if one is set, limited to 2048 characters.
    /// </summary>
    public string Text { get; } = jsonModel.Text;

    /// <summary>
    /// Points to an image, which is displayed in a small circular format to the left of the <see cref="Text"/>.
    /// </summary>
    public string? IconUrl { get; } = jsonModel.IconUrl;

    /// <summary>
    /// The URL of the icon image, proxied by the Discord CDN server.
    /// </summary>
    public string? ProxyIconUrl { get; } = jsonModel.ProxyIconUrl;
}

/// <summary>
/// Contains information used for the rendering and display of images in embeds.
/// </summary>
public class EmbedImage(JsonModels.JsonEmbedImage jsonModel)
{
    /// <summary>
    /// The URL of the image displayed in the embed.
    /// </summary>
    public string Url { get; } = jsonModel.Url;

    /// <summary>
    /// The URL of the image, proxied by the Discord CDN server.
    /// </summary>
    public string? ProxyUrl { get; } = jsonModel.ProxyUrl;

    /// <summary>
    /// The height of the image in pixels.
    /// </summary>
    public int? Height { get; } = jsonModel.Height;

    /// <summary>
    /// The width of the image in pixels.
    /// </summary>
    public int? Width { get; } = jsonModel.Width;

    /// <summary>
    /// The image's media type.
    /// </summary>
    public string? ContentType { get; } = jsonModel.ContentType;

    /// <summary>
    /// The thumbhash placeholder of the image.
    /// </summary>
    public string? Placeholder { get; } = jsonModel.Placeholder;

    /// <summary>
    /// The version of the placeholder.
    /// </summary>
    public int? PlaceholderVersion { get; } = jsonModel.PlaceholderVersion;

    /// <summary>
    /// Description (alt text) for the image.
    /// </summary>
    public string? Description { get; } = jsonModel.Description;

    /// <summary>
    /// Embed media flags combined as a bitfield.
    /// </summary>
    public EmbedMediaFlags? Flags { get; } = jsonModel.Flags;
}

/// <summary>
/// Contains information used for the rendering and display of videos in embeds.
/// </summary>
public class EmbedVideo(JsonModels.JsonEmbedVideo jsonModel)
{
    /// <summary>
    /// The URL of the video displayed in the embed.
    /// </summary>
    public string? Url { get; } = jsonModel.Url;

    /// <summary>
    /// The URL of the video displayed in the embed, proxied by the Discord CDN server.
    /// </summary>
    public string? ProxyUrl { get; } = jsonModel.ProxyUrl;

    /// <summary>
    /// The height of the video in pixels.
    /// </summary>
    public int? Height { get; } = jsonModel.Height;

    /// <summary>
    /// The width of the video in pixels.
    /// </summary>
    public int? Width { get; } = jsonModel.Width;

    /// <summary>
    /// The video's media type.
    /// </summary>
    public string? ContentType { get; } = jsonModel.ContentType;

    /// <summary>
    /// The thumbhash placeholder of the video.
    /// </summary>
    public string? Placeholder { get; } = jsonModel.Placeholder;

    /// <summary>
    /// The version of the placeholder.
    /// </summary>
    public int? PlaceholderVersion { get; } = jsonModel.PlaceholderVersion;

    /// <summary>
    /// Description (alt text) for the video.
    /// </summary>
    public string? Description { get; } = jsonModel.Description;

    /// <summary>
    /// Embed media flags combined as a bitfield.
    /// </summary>
    public EmbedMediaFlags? Flags { get; } = jsonModel.Flags;
}

[Flags]
public enum EmbedMediaFlags : sbyte
{
    IsAnimated = 1 << 5,
}

/// <summary>
/// Contains information used to display the provider tag at the top of an embed.
/// </summary>
public class EmbedProvider(JsonModels.JsonEmbedProvider jsonModel)
{
    /// <summary>
    /// The name of the provider, displayed at the top of the embed.
    /// </summary>
    public string? Name { get; } = jsonModel.Name;

    /// <summary>
    /// When set, turns the <see cref="Name"/> into a clickable link, pointing to the base of the specified URL.
    /// </summary>
    public string? Url { get; } = jsonModel.Url;
}

/// <summary>
/// Contains information about the author of an embed, used to render the author block.
/// </summary>
public class EmbedAuthor(JsonModels.JsonEmbedAuthor jsonModel)
{
    /// <summary>
    /// The name of the author, displayed next to the icon if one is specified.
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// When set, turns the name into a clickable link, pointing to the specified URL.
    /// </summary>
    public string? Url { get; } = jsonModel.Url;

    /// <summary>
    /// Points to an image, which is displayed in a small circular format to the left of the name.
    /// </summary>
    public string? IconUrl { get; } = jsonModel.IconUrl;

    /// <summary>
    /// The URL of the icon image, proxied by the Discord CDN server.
    /// </summary>
    public string? ProxyIconUrl { get; } = jsonModel.ProxyIconUrl;
}

/// <summary>
/// Contains information about an embed field, of which a maximum of 25 can be set per embed.
/// </summary>
public class EmbedField(JsonModels.JsonEmbedField jsonModel)
{
    /// <summary>
    /// Equivalent to <see cref="Embed.Title"/> but localised to a field, limited to 256 characters.
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// Equivalent to <see cref="Embed.Description"/> but localised to a field, limited to 1024 characters.
    /// </summary>
    public string Value { get; } = jsonModel.Value;

    /// <summary>
    /// When set alongside another field with <see cref="Inline"/> set, displays the fields side by side when supported.
    /// </summary>
    public bool Inline { get; } = jsonModel.Inline.GetValueOrDefault();
}

[Flags]
public enum EmbedFlags
{
    /// <summary>
    /// This embed is a fallback for a reply to an activity card.
    /// </summary>
    IsContentInventoryEntry = 1 << 5,
}
