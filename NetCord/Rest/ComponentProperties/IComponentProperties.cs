using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ComponentPropertiesConverter))]
public partial interface IComponentProperties
{
    /// <summary>
    /// Unique identifier for the component. Auto populated through increment if not provided.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// Type of the component.
    /// </summary>
    [JsonPropertyName("type")]
    public ComponentType ComponentType { get; }

    public class ComponentPropertiesConverter : JsonConverter<IComponentProperties>
    {
        private static readonly JsonEncodedText _type = JsonEncodedText.Encode("type");
        private static readonly JsonEncodedText _components = JsonEncodedText.Encode("components");
        private static readonly JsonEncodedText _id = JsonEncodedText.Encode("id");

        public override IComponentProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        private static void WriteActionRowStart(Utf8JsonWriter writer, int? actionRowId)
        {
            writer.WriteStartObject();

            writer.WriteNumber(_type, 1);

            if (actionRowId.HasValue)
                writer.WriteNumber(_id, actionRowId.GetValueOrDefault());

            writer.WritePropertyName(_components);
        }

        private static void WriteActionRowStartWithArrayStart(Utf8JsonWriter writer, int? actionRowId)
        {
            WriteActionRowStart(writer, actionRowId);

            writer.WriteStartArray();
        }

        public override void Write(Utf8JsonWriter writer, IComponentProperties component, JsonSerializerOptions options)
        {
            switch (component)
            {
                case ActionRowProperties actionRowProperties:
                    WriteActionRowStart(writer, actionRowProperties.Id);
                    JsonSerializer.Serialize(writer, actionRowProperties.Buttons, Serialization.Default.IEnumerableIButtonProperties);
                    writer.WriteEndObject();
                    return;
                case StringMenuProperties stringMenuProperties:
                    WriteActionRowStartWithArrayStart(writer, stringMenuProperties.ParentId);
                    JsonSerializer.Serialize(writer, stringMenuProperties, Serialization.Default.IStringMenuProperties);
                    break;
                case UserMenuProperties userMenuProperties:
                    WriteActionRowStartWithArrayStart(writer, userMenuProperties.ParentId);
                    JsonSerializer.Serialize(writer, userMenuProperties, Serialization.Default.UserMenuProperties);
                    break;
                case RoleMenuProperties roleMenuProperties:
                    WriteActionRowStartWithArrayStart(writer, roleMenuProperties.ParentId);
                    JsonSerializer.Serialize(writer, roleMenuProperties, Serialization.Default.RoleMenuProperties);
                    break;
                case MentionableMenuProperties mentionableMenuProperties:
                    WriteActionRowStartWithArrayStart(writer, mentionableMenuProperties.ParentId);
                    JsonSerializer.Serialize(writer, mentionableMenuProperties, Serialization.Default.MentionableMenuProperties);
                    break;
                case ChannelMenuProperties channelMenuProperties:
                    WriteActionRowStartWithArrayStart(writer, channelMenuProperties.ParentId);
                    JsonSerializer.Serialize(writer, channelMenuProperties, Serialization.Default.ChannelMenuProperties);
                    break;
                case TextInputProperties textInputProperties:
                    WriteActionRowStartWithArrayStart(writer, textInputProperties.ParentId);
                    JsonSerializer.Serialize(writer, textInputProperties, Serialization.Default.TextInputProperties);
                    break;
                case ComponentSectionProperties sectionProperties:
                    JsonSerializer.Serialize(writer, sectionProperties, Serialization.Default.IComponentSectionProperties);
                    return;
                case TextDisplayProperties textDisplayProperties:
                    JsonSerializer.Serialize(writer, textDisplayProperties, Serialization.Default.TextDisplayProperties);
                    return;
                // SectionThumbnailProperties is handled differently
                case MediaGalleryProperties mediaGalleryProperties:
                    JsonSerializer.Serialize(writer, mediaGalleryProperties, Serialization.Default.IMediaGalleryProperties);
                    return;
                case FileDisplayProperties fileDisplayProperties:
                    JsonSerializer.Serialize(writer, fileDisplayProperties, Serialization.Default.FileDisplayProperties);
                    return;
                case ComponentSeparatorProperties separatorProperties:
                    JsonSerializer.Serialize(writer, separatorProperties, Serialization.Default.ComponentSeparatorProperties);
                    return;
                case ComponentContainerProperties containerProperties:
                    JsonSerializer.Serialize(writer, containerProperties, Serialization.Default.IComponentContainerProperties);
                    return;
                default:
                    throw new InvalidOperationException($"Invalid '{nameof(IComponentProperties)}' value.");
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
