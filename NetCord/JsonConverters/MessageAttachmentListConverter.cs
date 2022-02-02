using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

internal class MessageAttachmentListConverter : JsonConverter<List<AttachmentProperties>>
{
    public override List<AttachmentProperties>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
    public override void Write(Utf8JsonWriter writer, List<AttachmentProperties> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        var span = CollectionsMarshal.AsSpan(value);
        int max = span.Length;
        for (int i = 0; i < max; i++)
        {
            var attachment = span[i];
            if (attachment.Description != null)
            {
                writer.WriteStartObject();
                writer.WriteNumber("id", i);
                writer.WriteString("description", attachment.Description);
                writer.WriteString("filename", attachment.FileName);
                writer.WriteEndObject();
            }
        }
        writer.WriteEndArray();
    }
}