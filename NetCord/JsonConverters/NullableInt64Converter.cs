using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

public class NullableInt64Converter : JsonConverter<ulong?>
{
    public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
        return Snowflake.Parse(span);
    }

    public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
    {
        var v = value.GetValueOrDefault();
        if (v == default)
            writer.WriteNullValue();
        else
            writer.WriteNumberValue(v);
    }

    public override ulong? ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override void WriteAsPropertyName(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
    {
        Span<byte> destination = stackalloc byte[20];
        Utf8Formatter.TryFormat(value.GetValueOrDefault(), destination, out int bytesWritten);
        writer.WritePropertyName(destination[..bytesWritten]);
    }
}
