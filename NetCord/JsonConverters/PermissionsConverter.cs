using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

internal class PermissionsConverter : JsonConverter<Permissions>
{
    public override Permissions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
        if (Utf8Parser.TryParse(span, out ulong value, out int bytesConsumed) && span.Length == bytesConsumed)
            return (Permissions)value;
        throw new FormatException("Either the JSON value is not in a supported format, or is out of bounds for a UInt64.");
    }

    public override void Write(Utf8JsonWriter writer, Permissions value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(((ulong)value).ToString());
    }
}
