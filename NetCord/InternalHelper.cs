namespace NetCord;

internal static class InternalHelper
{
    public static string GetImageExtension(ImageFormat format)
    {
        return format switch
        {
            ImageFormat.Jpeg => "jpg",
            ImageFormat.Png => "png",
            ImageFormat.WebP => "webp",
            ImageFormat.Gif => "gif",
            ImageFormat.Lottie => "json",
            _ => throw new ArgumentException("Invalid image format", nameof(format))
        };
    }
}