namespace NetCord.Rest;

public partial class GuildStickerProperties : IHttpSerializable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="attachment">Sticker attachment.</param>
    /// <param name="format">Sticker format.</param>
    /// <param name="tags">Sticker autocomplete/suggestion tags (max 200 characters long when comma joined).</param>
    public GuildStickerProperties(AttachmentProperties attachment, StickerFormat format, IEnumerable<string> tags)
    {
        if (!attachment.SupportsHttpSerialization)
            throw new ArgumentException("The attachment does not support HTTP serialization.", nameof(attachment));

        Attachment = attachment;
        Format = format;
        Tags = tags;
    }

    /// <summary>
    /// Sticker attachment.
    /// </summary>
    public AttachmentProperties Attachment { get; set; }

    /// <summary>
    /// Sticker format.
    /// </summary>
    public StickerFormat Format { get; set; }

    /// <summary>
    /// Sticker autocomplete/suggestion tags (max 200 characters long when comma joined).
    /// </summary>
    public IEnumerable<string> Tags { get; set; }

    public HttpContent Serialize()
    {
        var attachment = Attachment;
        var content = attachment.Serialize();
        content.Headers.ContentType = new(Format switch
        {
            StickerFormat.Png or StickerFormat.Apng => "image/png",
            StickerFormat.Lottie => "application/json",
            StickerFormat.Gif => "image/gif",
            _ => throw new System.ComponentModel.InvalidEnumArgumentException(null, (int)Format, typeof(StickerFormat)),
        });

        return new MultipartFormDataContent()
        {
            { new StringContent(attachment.FileName), "name" },
            { new StringContent(attachment.Description ?? string.Empty), "description" },
            { new StringContent(string.Join(',', Tags)), "tags" },
            { content, "file", "f" },
        };
    }
}
