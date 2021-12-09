using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonConverter(typeof(ComponentConverter))]
    public abstract class Component
    {
        [JsonPropertyName("type")]
        public MessageComponentType ComponentType { get; }

        protected Component(MessageComponentType type)
        {
            ComponentType = type;
        }

        private class ComponentConverter : JsonConverter<Component>
        {
            public override Component Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
            public override void Write(Utf8JsonWriter writer, Component component, JsonSerializerOptions options)
            {
                if (component is Menu menu)
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
                    var actionRow = (ActionRow)component;
                    JsonSerializer.Serialize(writer, actionRow);
                }
            }
        }
    }
}
