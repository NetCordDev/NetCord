using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonConverters;

internal class TextInputPropertiesIEnumerableConverter : JsonConverter<IEnumerable<TextInputProperties>>
{
    public override IEnumerable<TextInputProperties>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
    public override void Write(Utf8JsonWriter writer, IEnumerable<TextInputProperties> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var textInput in value)
        {
            writer.WriteStartObject();
            writer.WriteNumber("type", 1);
            writer.WriteStartArray("components");
            JsonSerializer.Serialize(writer, textInput, options);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
