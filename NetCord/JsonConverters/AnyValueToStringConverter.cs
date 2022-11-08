using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

internal class AnyValueToStringConverter : JsonConverter<string>
{
    private static readonly UTF8Encoding _encoding = new(false, true);

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.HasValueSequence ? _encoding.GetString(reader.ValueSequence) : _encoding.GetString(reader.ValueSpan);
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
