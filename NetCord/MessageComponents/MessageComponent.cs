using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonConverter(typeof(ComponentConverter))]
    public abstract class MessageComponent
    {
        [JsonPropertyName("type")]
        public ComponentType ComponentType { get; }

        protected MessageComponent(ComponentType type)
        {
            ComponentType = type;
        }

        private class ComponentConverter : JsonConverter<MessageComponent>
        {
            public override MessageComponent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
            public override void Write(Utf8JsonWriter writer, MessageComponent component, JsonSerializerOptions options)
            {
                if (component is MessageMenu menu)
                {
                    writer.WriteStartObject();
                    writer.WriteNumber("type", 1);
                    writer.WriteStartArray("components");
                    JsonSerializer.Serialize(writer, menu);
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }
                else
                {
                    var actionRow = (MessageActionRow)component;
                    JsonSerializer.Serialize(writer, actionRow);
                }
            }
        }
    }
}
