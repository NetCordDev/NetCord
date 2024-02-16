using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="type">Type of the component.</param>
[JsonConverter(typeof(ComponentConverter))]
public abstract partial class ComponentProperties(ComponentType type)
{
    /// <summary>
    /// Type of the component.
    /// </summary>
    [JsonPropertyName("type")]
    public ComponentType ComponentType { get; } = type;

    public class ComponentConverter : JsonConverter<ComponentProperties>
    {
        private static readonly JsonEncodedText _type = JsonEncodedText.Encode("type");
        private static readonly JsonEncodedText _components = JsonEncodedText.Encode("components");

        public override ComponentProperties Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, ComponentProperties component, JsonSerializerOptions options)
        {
            if (component is ActionRowProperties actionRowProperties)
            {
                JsonSerializer.Serialize(writer, actionRowProperties, Serialization.Default.ActionRowProperties);
                return;
            }

            writer.WriteStartObject();

            writer.WriteNumber(_type, 1);

            writer.WriteStartArray(_components);

            switch (component)
            {
                case StringMenuProperties stringMenuProperties:
                    JsonSerializer.Serialize(writer, stringMenuProperties, Serialization.Default.StringMenuProperties);
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
                default:
                    throw new InvalidOperationException($"Invalid {nameof(ComponentProperties)} value.");
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
