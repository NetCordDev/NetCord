using System.Text.Json;
using System.Text.Json.Serialization;

using NetCord.Rest;

namespace NetCord.JsonConverters;

internal class AttachmentPropertiesIEnumerableConverter : JsonConverter<IEnumerable<AttachmentProperties>>
{
    private static readonly JsonEncodedText _id = JsonEncodedText.Encode("id");
    private static readonly JsonEncodedText _fileName = JsonEncodedText.Encode("filename");
    private static readonly JsonEncodedText _description = JsonEncodedText.Encode("description");
    private static readonly JsonEncodedText _uploadedFileName = JsonEncodedText.Encode("uploaded_filename");

    public override IEnumerable<AttachmentProperties>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
    public override void Write(Utf8JsonWriter writer, IEnumerable<AttachmentProperties> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        int i = 0;
        foreach (var attachment in value)
        {
            writer.WriteStartObject();
            writer.WriteNumber(_id, i);
            writer.WriteString(_fileName, attachment.FileName);
            if (attachment.Description != null)
                writer.WriteString(_description, attachment.Description);
            if (attachment is GoogleCloudPlatformAttachmentProperties googleCloudPlatformAttachment)
                writer.WriteString(_uploadedFileName, googleCloudPlatformAttachment.UploadedFileName);
            writer.WriteEndObject();
            i++;
        }
        writer.WriteEndArray();
    }
}
