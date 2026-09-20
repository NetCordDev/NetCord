using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents an <see cref="Attachment"/> with properties relevant to image/video files.
/// </summary>
public class ImageAttachment(JsonModels.JsonAttachment jsonModel) : Attachment(jsonModel)
{
    /// <summary>
    /// The height of the attachment in pixels.
    /// </summary>
    public int Height => jsonModel.Height.GetValueOrDefault();

    /// <summary>
    /// The width of the attachment in pixels.
    /// </summary>
    public int Width => jsonModel.Width.GetValueOrDefault();

    /// <summary>
    /// The attachment's <see href="https://evanw.github.io/thumbhash/">thumbhash</see> placeholder.
    /// </summary>
    public string? Placeholder => jsonModel.Placeholder;

    /// <summary>
    /// The <see cref="Placeholder"/>'s version.
    /// </summary>
    public int PlaceholderVersion => jsonModel.PlaceholderVersion.GetValueOrDefault();
}
