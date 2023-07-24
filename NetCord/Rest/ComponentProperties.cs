using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ComponentConverter))]
public abstract partial class ComponentProperties
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
        private static readonly JsonEncodedText _type = JsonEncodedText.Encode("type");
        private static readonly JsonEncodedText _components = JsonEncodedText.Encode("components");

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

                    writer.WriteNumber(_type, 1);

                    writer.WriteStartArray(_components);
                    switch (component)
                    {
                        case StringMenuProperties stringMenuProperties:
                            JsonSerializer.Serialize(writer, stringMenuProperties, StringMenuProperties.StringMenuPropertiesSerializerContext.WithOptions.StringMenuProperties);
                            break;
                        case UserMenuProperties userMenuProperties:
                            JsonSerializer.Serialize(writer, userMenuProperties, UserMenuProperties.UserMenuPropertiesSerializerContext.WithOptions.UserMenuProperties);
                            break;
                        case RoleMenuProperties roleMenuProperties:
                            JsonSerializer.Serialize(writer, roleMenuProperties, RoleMenuProperties.RoleMenuPropertiesSerializerContext.WithOptions.RoleMenuProperties);
                            break;
                        case MentionableMenuProperties mentionableMenuProperties:
                            JsonSerializer.Serialize(writer, mentionableMenuProperties, MentionableMenuProperties.MentionableMenuPropertiesSerializerContext.WithOptions.MentionableMenuProperties);
                            break;
                        case ChannelMenuProperties channelMenuProperties:
                            JsonSerializer.Serialize(writer, channelMenuProperties, ChannelMenuProperties.ChannelMenuPropertiesSerializerContext.WithOptions.ChannelMenuProperties);
                            break;
                        default:
                            throw new InvalidOperationException($"Invalid {nameof(ComponentProperties)} value.");
                    }
                    writer.WriteEndArray();

                    writer.WriteEndObject();
                    break;
            }
        }
    }
}
