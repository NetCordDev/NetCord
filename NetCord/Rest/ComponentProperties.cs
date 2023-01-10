using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ComponentConverter))]
public abstract class ComponentProperties
{
    /// <summary>
    /// Type of the component.
    /// </summary>
    [JsonPropertyName("type")]
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">Type of the component.</param>
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
                case ComponentType.ActionRow:
                    JsonSerializer.Serialize(writer, (ActionRowProperties)component, ActionRowProperties.ActionRowPropertiesSerializerContext.WithOptions.ActionRowProperties);
                    break;
                default:
                    writer.WriteStartObject();
                    writer.WriteNumber("type", 1);
                    writer.WriteStartArray("components");
                    switch (component.ComponentType)
                    {
                        case ComponentType.StringMenu:
                            JsonSerializer.Serialize(writer, (StringMenuProperties)component, StringMenuProperties.StringMenuPropertiesSerializerContext.WithOptions.StringMenuProperties);
                            break;
                        case ComponentType.UserMenu:
                            JsonSerializer.Serialize(writer, (UserMenuProperties)component, UserMenuProperties.UserMenuPropertiesSerializerContext.WithOptions.UserMenuProperties);
                            break;
                        case ComponentType.RoleMenu:
                            JsonSerializer.Serialize(writer, (RoleMenuProperties)component, RoleMenuProperties.RoleMenuPropertiesSerializerContext.WithOptions.RoleMenuProperties);
                            break;
                        case ComponentType.MentionableMenu:
                            JsonSerializer.Serialize(writer, (MentionableMenuProperties)component, MentionableMenuProperties.MentionableMenuPropertiesSerializerContext.WithOptions.MentionableMenuProperties);
                            break;
                        case ComponentType.ChannelMenu:
                            JsonSerializer.Serialize(writer, (ChannelMenuProperties)component, ChannelMenuProperties.ChannelMenuPropertiesSerializerContext.WithOptions.ChannelMenuProperties);
                            break;
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    break;
            }
        }
    }
}
