namespace NetCord;

/// <summary>
/// Displays embedded content such as an image or URL, alongside a title and various other fields. You can only have up to 10 embeds per message, and the total text of all embeds must be less than or equal to 6000 characters.
/// </summary>
public class Embed : IJsonModel<JsonModels.JsonEmbed>
{
    JsonModels.JsonEmbed IJsonModel<JsonModels.JsonEmbed>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbed _jsonModel;
    
    /// <summary>
    /// The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <see cref="Url"/>, has a 256 character limit.
    /// </summary>
    public string? Title => _jsonModel.Title;
    
    /// <summary>
    /// Information about the type of the embed.
    /// </summary>
    public EmbedType? Type => _jsonModel.Type;
    
    /// <summary>
    /// The part of the embed where the main text is contained, limited to 4096 characters.
    /// </summary>
    public string? Description => _jsonModel.Description;
    
    /// <summary>
    /// A link to an address of a webpage. When set, the <see cref="Title"/> becomes a clickable link, directing to the URL.
    /// </summary>
    public string? Url => _jsonModel.Url;
    
    /// <summary>
    /// Displays time in a format similar to a message timestamp. Located next to the footer.
    /// </summary>
    public DateTimeOffset? Timestamp => _jsonModel.Timestamp;
    
    /// <summary>
    /// The color of the embed’s border in an RGB format.
    /// </summary>
    public Color? Color => _jsonModel.Color;
    
    /// <summary>
    /// The text at the bottom of the embed, limited to 2048 characters.
    /// </summary>
    public EmbedFooter? Footer { get; }
    
    /// <summary>
    /// The image included in the embed, displayed as a large-sized image located below the <see cref="Description"/> element.
    /// </summary>
    public EmbedImage? Image { get; }
    
    /// <summary>
    /// The thumbnail of the embed, displayed as a medium-sized image in the top right corner of the embed.
    /// </summary>
    public EmbedThumbnail? Thumbnail { get; }
    
    /// <summary>
    /// The video included and displayed in the embed.
    /// </summary>
    public EmbedVideo? Video { get; }
    
    /// <summary>
    /// The provider of the embed content (YouTube, Twitter/X, etc), automatically generated from links in content.
    /// </summary>
    public EmbedProvider? Provider { get; }
    
    /// <summary>
    /// Contains the author block of the embed, always located at the top of the embed.
    /// </summary>
    public EmbedAuthor? Author { get; }
    
    /// <summary>
    /// Allows the addition of multiple subtitles with additional content underneath them below the main <see cref="Title"/> and <see cref="Description"/> blocks, maximum of 25 per embed.
    /// </summary>
    public IReadOnlyList<EmbedField> Fields { get; }
    
    /// <summary>
    /// Creates an embed from its <see cref="JsonModels.JsonEmbed"/> equivalent.
    /// </summary>
    public Embed(JsonModels.JsonEmbed jsonModel)
    {
        _jsonModel = jsonModel;

        var footer = jsonModel.Footer;
        if (footer is not null)
            Footer = new(footer);

        var image = jsonModel.Image;
        if (image is not null)
            Image = new(image);

        var thumbnail = jsonModel.Thumbnail;
        if (thumbnail is not null)
            Thumbnail = new(thumbnail);

        var video = jsonModel.Video;
        if (video is not null)
            Video = new(video);

        var provider = jsonModel.Provider;
        if (provider is not null)
            Provider = new(provider);

        var author = jsonModel.Author;
        if (author is not null)
            Author = new(author);

        Fields = jsonModel.Fields.SelectOrEmpty(f => new EmbedField(f)).ToArray();
    }
}
