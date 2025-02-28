using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ComponentPropertiesConverter))]
public abstract partial class ComponentProperties
{
    /// <summary>
    /// Type of the component.
    /// </summary>
    [JsonPropertyName("type")]
    public abstract ComponentType ComponentType { get; }

    public class ComponentPropertiesConverter : JsonConverter<ComponentProperties>
    {
        private static readonly JsonEncodedText _type = JsonEncodedText.Encode("type");
        private static readonly JsonEncodedText _components = JsonEncodedText.Encode("components");

        public override ComponentProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        private static void WriteActionRowStart(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WriteNumber(_type, 1);

            writer.WritePropertyName(_components);
        }

        private static void WriteActionRowStartWithArrayStart(Utf8JsonWriter writer)
        {
            WriteActionRowStart(writer);

            writer.WriteStartArray();
        }

        public override void Write(Utf8JsonWriter writer, ComponentProperties component, JsonSerializerOptions options)
        {
            switch (component)
            {
                case ActionRowProperties actionRowProperties:
                    WriteActionRowStart(writer);
                    JsonSerializer.Serialize(writer, actionRowProperties.Buttons, Serialization.Default.IEnumerableIButtonProperties);
                    return;
                case StringMenuProperties stringMenuProperties:
                    WriteActionRowStartWithArrayStart(writer);
                    JsonSerializer.Serialize(writer, stringMenuProperties, Serialization.Default.IStringMenuProperties);
                    break;
                case UserMenuProperties userMenuProperties:
                    WriteActionRowStartWithArrayStart(writer);
                    JsonSerializer.Serialize(writer, userMenuProperties, Serialization.Default.UserMenuProperties);
                    break;
                case RoleMenuProperties roleMenuProperties:
                    WriteActionRowStartWithArrayStart(writer);
                    JsonSerializer.Serialize(writer, roleMenuProperties, Serialization.Default.RoleMenuProperties);
                    break;
                case MentionableMenuProperties mentionableMenuProperties:
                    WriteActionRowStartWithArrayStart(writer);
                    JsonSerializer.Serialize(writer, mentionableMenuProperties, Serialization.Default.MentionableMenuProperties);
                    break;
                case ChannelMenuProperties channelMenuProperties:
                    WriteActionRowStartWithArrayStart(writer);
                    JsonSerializer.Serialize(writer, channelMenuProperties, Serialization.Default.ChannelMenuProperties);
                    break;
                case TextInputProperties textInputProperties:
                    WriteActionRowStartWithArrayStart(writer);
                    JsonSerializer.Serialize(writer, textInputProperties, Serialization.Default.TextInputProperties);
                    break;
                case SectionProperties sectionProperties:
                    JsonSerializer.Serialize(writer, sectionProperties, Serialization.Default.ISectionProperties);
                    return;
                case TextDisplayProperties textDisplayProperties:
                    JsonSerializer.Serialize(writer, textDisplayProperties, Serialization.Default.TextDisplayProperties);
                    return;
                case ThumbnailProperties thumbnailProperties:
                    JsonSerializer.Serialize(writer, thumbnailProperties, Serialization.Default.ThumbnailProperties);
                    return;
                case MediaGalleryProperties mediaGalleryProperties:
                    JsonSerializer.Serialize(writer, mediaGalleryProperties, Serialization.Default.IMediaGalleryProperties);
                    return;
                case FileDisplayProperties fileDisplayProperties:
                    JsonSerializer.Serialize(writer, fileDisplayProperties, Serialization.Default.FileDisplayProperties);
                    return;
                case SeparatorProperties separatorProperties:
                    JsonSerializer.Serialize(writer, separatorProperties, Serialization.Default.SeparatorProperties);
                    return;
                case ContainerProperties containerProperties:
                    JsonSerializer.Serialize(writer, containerProperties, Serialization.Default.IContainerProperties);
                    return;
                default:
                    throw new InvalidOperationException($"Invalid '{nameof(ComponentProperties)}' value.");
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
