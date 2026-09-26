namespace NetCord;

/// <summary>
/// A bitfield with additional information about an attachment.
/// </summary>
[Flags]
public enum AttachmentFlags
{
    /// <summary>
    /// The attachment is a clip from a stream.
    /// </summary>
    Clip = 1 << 0,

    /// <summary>
    /// The attachment is the thumbnail of a media channel thread.
    /// </summary>
    /// <remarks>
    /// Media channel thumbnails are displayed in the channel grid, but not on the thread message.
    /// </remarks>
    Thumbnail = 1 << 1,

    /// <summary>
    /// The attachment has been edited using the remix feature on mobile (deprecated).
    /// </summary>
    Remix = 1 << 2,

    /// <summary>
    /// The attachment was marked as a spoiler, and remains blurred until clicked.
    /// </summary>
    Spoiler = 1 << 3,

    /// <summary>
    /// The attachment is an animated image.
    /// </summary>
    Animated = 1 << 5
}
