using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

public abstract class MenuPropertiesDefaultValuesConverter(JsonEncodedText typeValue) : JsonConverter<IEnumerable<ulong>>
{
    private static readonly JsonEncodedText _id = JsonEncodedText.Encode("id");
    private static readonly JsonEncodedText _type = JsonEncodedText.Encode("type");

    public override IEnumerable<ulong>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.ToObject(Serialization.Default.JsonEntityArray).Select(x => x.Id);
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<ulong> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var id in value)
        {
            writer.WriteStartObject();

            writer.WriteNumber(_id, id);
            writer.WriteString(_type, typeValue);

            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }
}
