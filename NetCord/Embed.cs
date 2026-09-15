using System.Linq;

namespace NetCord;

/// <summary>
/// Displays embedded content such as an image or URL, alongside a title and various other fields. You can only have up to 10 embeds per message, and the total text of all embeds must be less than or equal to 6000 characters.
/// </summary>
public class Embed(JsonModels.JsonEmbed jsonModel) : IJsonModel<JsonModels.JsonEmbed>
{
    JsonModels.JsonEmbed IJsonModel<JsonModels.JsonEmbed>.JsonModel => jsonModel;

    /// <summary>
    /// The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <see cref="Url"/>, has a 256 character limit.
    /// </summary>
    public string? Title => jsonModel.Title;

    /// <summary>
    /// Information about the type of the embed.
    /// </summary>
    public EmbedType? Type => jsonModel.Type;

    /// <summary>
    /// The part of the embed where the main text is contained, limited to 4096 characters.
    /// </summary>
    public string? Description => jsonModel.Description;

    /// <summary>
    /// A link to an address of a webpage. When set, the <see cref="Title"/> becomes a clickable link, directing to the URL.
    /// </summary>
    public string? Url => jsonModel.Url;

    /// <summary>
    /// Displays time in a format similar to a message timestamp. Located next to the <see cref="Footer"/>.
    /// </summary>
    public DateTimeOffset? Timestamp => jsonModel.Timestamp;

    /// <summary>
    /// The color of the embed's border in an RGB format.
    /// </summary>
    public Color? Color => jsonModel.Color;

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
    public EmbedThumbnail? Thumbnail { get; } = jsonModel.Thumbnail is { } thumbnail ? new(thumbnail) : null;

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
    public IReadOnlyList<EmbedField> Fields { get; } = jsonModel.Fields.SelectOrEmpty(f => new EmbedField(f)).ToArray();
}
