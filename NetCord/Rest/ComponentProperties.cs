using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ComponentConverter))]
public abstract class ComponentProperties
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType { get; }

    protected ComponentProperties(ComponentType type)
    {
        ComponentType = type;
    }

    internal class ComponentConverter : JsonConverter<ComponentProperties>
    {
        public override ComponentProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();
        public override void Write(Utf8JsonWriter writer, ComponentProperties component, JsonSerializerOptions options)
        {
            switch (component.ComponentType)
            {
                case ComponentType.Menu:
                    writer.WriteStartObject();
                    writer.WriteNumber("type", 1);
                    writer.WriteStartArray("components");
                    JsonSerializer.Serialize(writer, (MenuProperties)component, MenuProperties.MenuPropertiesSerializerContext.WithOptions.MenuProperties);
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
                case ComponentType.ActionRow:
                    JsonSerializer.Serialize(writer, (ActionRowProperties)component, ActionRowProperties.ActionRowPropertiesSerializerContext.WithOptions.ActionRowProperties);
                    break;
            }
        }
    }
}
