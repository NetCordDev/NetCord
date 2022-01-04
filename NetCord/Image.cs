using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(ImageConverter))]
public struct Image
{
    public ReadOnlyMemory<byte>? Bytes { get; }
    public ImageFormat? Format { get; }

    public Image(ReadOnlyMemory<byte> bytes, ImageFormat format)
    {
        Bytes = bytes;
        Format = format;
    }

    public static Image Empty => default;

    private class ImageConverter : JsonConverter<Image>
    {
        public override Image Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, Image value, JsonSerializerOptions options)
        {
            if (value.Bytes.HasValue)
                writer.WriteStringValue($"data:image/{InternalHelper.GetImageExtension(value.Format.GetValueOrDefault())};base64,{Convert.ToBase64String(value.Bytes.GetValueOrDefault().Span)}");
            else
                writer.WriteNullValue();
        }
    }
}