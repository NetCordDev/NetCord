using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonConverters;

public class AttachmentPropertiesIEnumerableConverter : JsonConverter<IEnumerable<AttachmentProperties>>
{
    public override IEnumerable<AttachmentProperties>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, IEnumerable<AttachmentProperties> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        int id = 0;
        foreach (var attachment in value)
        {
            attachment.WriteTo(writer, id);

            id++;
        }

        writer.WriteEndArray();
    }
}
