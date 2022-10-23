using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

internal class UInt64Converter : JsonConverter<ulong>
{
    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
        if (Utf8Parser.TryParse(span, out ulong value, out int bytesConsumed) && span.Length == bytesConsumed)
            return value;
        throw new FormatException("Either the JSON value is not in a supported format, or is out of bounds for a UInt64.");
    }

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
    {
        if (value == default)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(value);
    }

    public override ulong ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override void WriteAsPropertyName(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
    {
        Span<byte> destination = stackalloc byte[20];
        Utf8Formatter.TryFormat(value, destination, out int bytesWritten);
        writer.WritePropertyName(destination[..bytesWritten]);
    }
}
