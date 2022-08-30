using System.Text.Json.Serialization;
using System.Text.Json;

namespace NetCord.JsonConverters;

internal class NullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        var v = value.GetValueOrDefault();
        if (v == default)
            writer.WriteNullValue();
        else
            JsonSerializer.Serialize(writer, v, options);
    }
}
