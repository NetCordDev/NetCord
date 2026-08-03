namespace NetCord;

/// <summary>
/// Specifies a sticker's image format.
/// </summary>
public enum StickerFormat
{
    /// <summary>
    /// An image in the PNG format.
    /// </summary>
    PNG = 1,

    /// <summary>
    /// An animated image in the APNG format.
    /// </summary>
    APNG = 2,

    /// <summary>
    /// An animated image in the Lottie format. Rarely available.
    /// </summary>
    Lottie = 3,

    /// <summary>
    /// An animated image in the GIF format.
    /// </summary>
    GIF = 4,
}
