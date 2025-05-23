using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(ComponentSectionAccessoryPropertiesConverter))]
public partial interface IComponentSectionAccessoryProperties
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

    public class ComponentSectionAccessoryPropertiesConverter : JsonConverter<IComponentSectionAccessoryProperties>
    {
        public override IComponentSectionAccessoryProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, IComponentSectionAccessoryProperties value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case IButtonProperties buttonProperties:
                    JsonSerializer.Serialize(writer, buttonProperties, Serialization.Default.IButtonProperties);
                    break;
                case ComponentSectionThumbnailProperties thumbnailProperties:
                    JsonSerializer.Serialize(writer, thumbnailProperties, Serialization.Default.ComponentSectionThumbnailProperties);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid '{nameof(IComponentSectionAccessoryProperties)}' value.");
            }
        }
    }
}
