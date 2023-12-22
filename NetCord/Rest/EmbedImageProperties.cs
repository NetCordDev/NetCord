using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class EmbedImageProperties
{
    /// <summary>
    /// Url of the image.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

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
}
