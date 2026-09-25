namespace NetCord;

/// <summary>
/// Specifies the format of an image.
/// </summary>
public enum ImageFormat : byte
{
    /// <summary>
    /// An image in the JPEG format.
    /// </summary>
    Jpeg,

    /// <summary>
    /// An image in the PNG format.
    /// </summary>
    Png,

    /// <summary>
    /// An image in the WebP format, potentially animated.
    /// </summary>
    Webp,

    /// <summary>
    /// An animated image in the GIF format.
    /// </summary>
    Gif,

    /// <summary>
    /// An animated image in the Lottie format. Rarely available.
    /// </summary>
    Lottie,
}
