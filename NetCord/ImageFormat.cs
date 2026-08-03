namespace NetCord;

/// <summary>
/// Specifies the format of an image.
/// </summary>
public enum ImageFormat : byte
{
    /// <summary>
    /// An image in the JPEG format.
    /// </summary>
    JPEG,

    /// <summary>
    /// An image in the PNG format.
    /// </summary>
    PNG,

    /// <summary>
    /// An image in the WebP format, potentially animated.
    /// </summary>
    WebP,

    /// <summary>
    /// An animated image in the GIF format.
    /// </summary>
    GIF,

    /// <summary>
    /// An animated image in the Lottie format. Rarely available.
    /// </summary>
    Lottie,
}
