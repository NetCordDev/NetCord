using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ImagePropertiesConverter))]
public struct ImageProperties
{
    /// <summary>
    /// The format of the image.
    /// </summary>
    public ImageFormat Format { get; set; }

    /// <summary>
    /// The data of the image.
    /// </summary>
    public ReadOnlyMemory<byte> Data { get; set; }

    /// <summary>
    /// Whether <see cref="Data"/> is in Base64 format.
    /// </summary>
    public bool IsBase64 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format">The format of the image.</param>
    /// <param name="data">The data of the image.</param>
    /// <param name="isBase64">Whether <paramref name="data"/> is in Base64 format.</param>
    public ImageProperties(ImageFormat format, ReadOnlyMemory<byte> data, bool isBase64 = false)
    {
        Format = format;
        Data = data;
        IsBase64 = isBase64;
    }

    /// <summary>
    /// An empty <see cref="ImageProperties"/> instance.
    /// </summary>
    public static ImageProperties Empty => default;

    internal class ImagePropertiesConverter : JsonConverter<ImageProperties>
    {
        public override ImageProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, ImageProperties value, JsonSerializerOptions options)
        {
            var data = value.Data.Span;

            if (Unsafe.IsNullRef(ref MemoryMarshal.GetReference(data)))
                writer.WriteNullValue();
            else
            {
                var format = ImageUrl.GetFormatBytes(value.Format);
                var formatLength = format.Length;
                var isBase64 = value.IsBase64;
                var dataLength = data.Length;

                // data:image/ - 11
                // ;base64,    -  8
                var length = 11 + 8 + formatLength + (isBase64 ? dataLength : Base64.GetMaxEncodedToUtf8Length(dataLength));

                byte[]? toReturn = null;
                Span<byte> result = length <= 256 ? stackalloc byte[256] : (toReturn = ArrayPool<byte>.Shared.Rent(length));

                "data:image/"u8.CopyTo(result);
                format.CopyTo(result[11..]);
                ";base64,"u8.CopyTo(result[(11 + formatLength)..]);

                int bytesWritten;
                if (isBase64)
                {
                    data.CopyTo(result[(11 + 8 + formatLength)..]);
                    bytesWritten = dataLength;
                }
                else
                    Base64.EncodeToUtf8(data, result[(11 + 8 + formatLength)..], out _, out bytesWritten);

                writer.WriteStringValue(result[..(11 + 8 + formatLength + bytesWritten)]);

                if (toReturn is not null)
                    ArrayPool<byte>.Shared.Return(toReturn);
            }
        }
    }
}
