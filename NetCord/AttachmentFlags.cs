namespace NetCord;

[Flags]
public enum AttachmentFlags
{
    /// <summary>
    /// This attachment is a clip.
    /// </summary>
    IsClip = 1 << 0,

    /// <summary>
    /// This attachment is a thumbnail.
    /// </summary>
    IsThumbnail = 1 << 1,

    /// <summary>
    /// This attachment has been edited using the remix feature on mobile.
    /// </summary>
    IsRemix = 1 << 2,
}
