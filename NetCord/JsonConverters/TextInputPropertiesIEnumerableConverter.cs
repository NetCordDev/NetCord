using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonConverters;

public class TextInputPropertiesIEnumerableConverter : JsonConverter<IEnumerable<TextInputProperties>>
{
    private static readonly JsonEncodedText _type = JsonEncodedText.Encode("type");
    private static readonly JsonEncodedText _components = JsonEncodedText.Encode("components");

    public override IEnumerable<TextInputProperties>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
    public override void Write(Utf8JsonWriter writer, IEnumerable<TextInputProperties> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var textInput in value)
        {
            writer.WriteStartObject();
            writer.WriteNumber(_type, 1);
            writer.WriteStartArray(_components);
            JsonSerializer.Serialize(writer, textInput, TextInputProperties.TextInputPropertiesSerializerContext.WithOptions.TextInputProperties);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
    }
}
