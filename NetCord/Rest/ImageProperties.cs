using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ImagePropertiesConverter))]
public struct ImageProperties
{
    public ReadOnlyMemory<byte> Bytes { get; set; }
    public ImageFormat Format { get; set; }

    public ImageProperties(ReadOnlyMemory<byte> bytes, ImageFormat format)
    {
        Bytes = bytes;
        Format = format;
    }

    public static ImageProperties Empty => default;

    internal class ImagePropertiesConverter : JsonConverter<ImageProperties>
    {
        public override ImageProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, ImageProperties value, JsonSerializerOptions options)
        {
            var valueBytes = value.Bytes;
            if (valueBytes.IsEmpty)
                writer.WriteNullValue();
            else
            {
                var format = ImageUrl.GetFormatBytes(value.Format);
                var formatLength = format.Length;

                // data:image/ - 11
                // ;base64,    -  8
                var length = 11 + 8 + formatLength + Base64.GetMaxEncodedToUtf8Length(valueBytes.Length);

                byte[]? toReturn = null;
                Span<byte> bytes = length <= 256 ? stackalloc byte[256] : (toReturn = ArrayPool<byte>.Shared.Rent(length));

                "data:image/"u8.CopyTo(bytes);
                format.CopyTo(bytes[11..]);
                ";base64,"u8.CopyTo(bytes[(11 + formatLength)..]);
                Base64.EncodeToUtf8(valueBytes.Span, bytes[(11 + 8 + formatLength)..], out _, out var bytesWritten);

                writer.WriteStringValue(bytes[..(11 + 8 + formatLength + bytesWritten)]);

                if (toReturn is not null)
                    ArrayPool<byte>.Shared.Return(toReturn);
            }
        }
    }
}
