using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

internal class AttachmentPropertiesIEnumerableConverter : JsonConverter<IEnumerable<AttachmentProperties>>
{
    public override IEnumerable<AttachmentProperties>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
    public override void Write(Utf8JsonWriter writer, IEnumerable<AttachmentProperties> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        int i = 0;
        foreach (var attachment in value)
        {
            if (attachment.Description != null)
            {
                writer.WriteStartObject();
                writer.WriteNumber("id", i);
                writer.WriteString("description", attachment.Description);
                writer.WriteString("filename", attachment.FileName);
                writer.WriteEndObject();
            }
            i++;
        }
        writer.WriteEndArray();
    }
}