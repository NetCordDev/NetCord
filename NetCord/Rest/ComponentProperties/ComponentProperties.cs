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

        public override void Write(Utf8JsonWriter writer, ComponentProperties component, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber(_type, 1);

            if (component is ActionRowProperties actionRowProperties)
            {
                writer.WritePropertyName(_components);
                JsonSerializer.Serialize(writer, actionRowProperties.Buttons, Serialization.Default.IEnumerableIButtonProperties);
            }
            else
            {
                writer.WriteStartArray(_components);

                switch (component)
                {
                    case StringMenuProperties stringMenuProperties:
                        JsonSerializer.Serialize(writer, stringMenuProperties, Serialization.Default.IStringMenuProperties);
                        break;
                    case UserMenuProperties userMenuProperties:
                        JsonSerializer.Serialize(writer, userMenuProperties, Serialization.Default.UserMenuProperties);
                        break;
                    case RoleMenuProperties roleMenuProperties:
                        JsonSerializer.Serialize(writer, roleMenuProperties, Serialization.Default.RoleMenuProperties);
                        break;
                    case MentionableMenuProperties mentionableMenuProperties:
                        JsonSerializer.Serialize(writer, mentionableMenuProperties, Serialization.Default.MentionableMenuProperties);
                        break;
                    case ChannelMenuProperties channelMenuProperties:
                        JsonSerializer.Serialize(writer, channelMenuProperties, Serialization.Default.ChannelMenuProperties);
                        break;
                    case TextInputProperties textInputProperties:
                        JsonSerializer.Serialize(writer, textInputProperties, Serialization.Default.TextInputProperties);
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid '{nameof(ComponentProperties)}' value.");
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}
