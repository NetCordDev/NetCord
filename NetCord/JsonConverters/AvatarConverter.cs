using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters
{
    class AvatarConverter : JsonConverter<SelfUser.Avatar>
    {
        public override SelfUser.Avatar Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, SelfUser.Avatar value, JsonSerializerOptions options)
        {
            writer.WriteStringValue("data:" + value.ContentType.MediaType + ";base64," + value.AvatarBase64);
        }
    }
}
