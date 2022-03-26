using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(ComponentConverter))]
public abstract class ComponentProperties
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType { get; }

    protected ComponentProperties(ComponentType type)
    {
        ComponentType = type;
    }

    private class ComponentConverter : JsonConverter<ComponentProperties>
    {
        public override ComponentProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, ComponentProperties component, JsonSerializerOptions options)
        {
            if (component is MenuProperties menu)
            {
                writer.WriteStartObject();
                writer.WriteNumber("type", 1);
                writer.WriteStartArray("components");
                JsonSerializer.Serialize(writer, menu, options);
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
            else
            {
                var actionRow = (ActionRowProperties)component;
                JsonSerializer.Serialize(writer, actionRow, options);
            }
        }
    }
}