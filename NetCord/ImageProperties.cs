using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(ImagePropertiesConverter))]
public readonly struct ImageProperties
{
    public ReadOnlyMemory<byte> Bytes { get; }
    public ImageFormat Format { get; }

    public ImageProperties(ReadOnlyMemory<byte> bytes, ImageFormat format)
    {
        Bytes = bytes;
        Format = format;
    }

    public static ImageProperties Empty => default;

    private class ImagePropertiesConverter : JsonConverter<ImageProperties>
    {
        public override ImageProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, ImageProperties value, JsonSerializerOptions options)
        {
            if (value.Bytes.IsEmpty)
                writer.WriteNullValue();
            else
                writer.WriteStringValue($"data:image/{GetFormat(value.Format)};base64,{Convert.ToBase64String(value.Bytes.Span)}");
        }
    }

    internal static string GetFormat(ImageFormat format)
    {
        return format switch
        {
            ImageFormat.Jpeg => "jpg",
            ImageFormat.Png => "png",
            ImageFormat.WebP => "webp",
            ImageFormat.Gif => "gif",
            ImageFormat.Lottie => "json",
            _ => throw new System.ComponentModel.InvalidEnumArgumentException("Invalid image format")
        };
    }
}