using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

internal class NonceConverter : JsonConverter<string>
{
    private static readonly UTF8Encoding _encoding = new(false, true);

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return _encoding.GetString(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan);
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
