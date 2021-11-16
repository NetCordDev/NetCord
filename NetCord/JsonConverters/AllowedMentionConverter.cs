using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters
{
    internal class AllowedMentionConverter : JsonConverter<AllowedMentions>
    {
        public override AllowedMentions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, AllowedMentions value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            List<string> list = new();
            if (value.TheOnlyAllowedRoles == null)
                list.Add("roles");
            else
            {
                writer.WritePropertyName("roles");
                JsonSerializer.Serialize(writer, value.TheOnlyAllowedRoles);
            }
            if (value.TheOnlyAllowedUsers == null)
                list.Add("users");
            else
            {
                writer.WritePropertyName("users");
                JsonSerializer.Serialize(writer, value.TheOnlyAllowedUsers);
            }
            if (value.Everyone)
                list.Add("everyone");
            writer.WritePropertyName("parse");
            JsonSerializer.Serialize(writer, list);
            writer.WritePropertyName("replied_user");
            JsonSerializer.Serialize(writer, value.ReplyMention);
            writer.WriteEndObject();
        }
    }
}