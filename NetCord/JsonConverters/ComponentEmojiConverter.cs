using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters
{
    internal class ComponentEmojiConverter : JsonConverter<DiscordId>
    {
        public override DiscordId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, DiscordId value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("id", value.ToString());
            writer.WriteEndObject();
        }
    }
}