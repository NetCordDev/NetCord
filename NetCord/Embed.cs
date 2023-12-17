namespace NetCord;

/// <summary>
/// Displays embedded content such as an image or URL, alongside a title and various other fields. All text in the embed must add up to under 6000 characters, and you can only have up to 10 embeds per message.
/// </summary>
public class Embed : IJsonModel<JsonModels.JsonEmbed>
{
    JsonModels.JsonEmbed IJsonModel<JsonModels.JsonEmbed>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbed _jsonModel;
    
    /// <summary>
    /// The text that is placed above the description, usually highlighted. Also directs to a URL if given, has a 256 character limit.
    /// </summary>
    public string? Title => _jsonModel.Title;
    
    /// <summary>
    /// The type of the embed, always set to <see cref="EmbedType.Rich"/> for webhook embeds.
    /// </summary>
    public EmbedType? Type => _jsonModel.Type;
    
    /// <summary>
    /// The part of the embed where most of the text is contained, limited to 4096 characters.
    /// </summary>
    public string? Description => _jsonModel.Description;
    
    /// <summary>
    /// The link to the address of the webpage. Mostly used with the thumbnail, icon and author elements in order to link to an image.
    /// </summary>
    public string? Url => _jsonModel.Url;
    
    /// <summary>
    /// Time that the embed was posted. Located next to the footer.
    /// </summary>
    public DateTimeOffset? Timestamp => _jsonModel.Timestamp;
    
    /// <summary>
    /// Color of your embed’s border, usually in hexadecimal or decimal, specified by an integer between 0x000000 and 0xFFFFFF.
    /// </summary>
    public Color? Color => _jsonModel.Color;
    
    
    /// <summary>
    /// Text at the bottom of the embed, limited to 2048 characters.
    /// </summary>
    public EmbedFooter? Footer { get; }
    
    /// <summary>
    /// The URL of the image, a large-sized image located below the "Description" element.
    /// </summary>
    public EmbedImage? Image { get; }
    
    /// <summary>
    /// The URL of the thumbnail, a medium-sized image in the top right corner of the embed.
    /// </summary>
    public EmbedThumbnail? Thumbnail { get; }
    
    /// <summary>
    /// The URL of the video to include in the embed.
    /// </summary>
    public EmbedVideo? Video { get; }
    
    /// <summary>
    /// The provider of the embed content (YouTube, Twitter/X, etc). Generally unused in bot embeds.
    /// </summary>
    public EmbedProvider? Provider { get; }
    
    /// <summary>
    /// Adds the author block to the embed, always located at the top of the embed.
    /// </summary>
    public EmbedAuthor? Author { get; }
    
    /// <summary>
    /// Allows you to add multiple subtitles with additional content underneath them below the main <see cref="Title"/> and <see cref="Description"/> blocks, maximum of 25 per embed.
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
