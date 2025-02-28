using System.Collections;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class SectionProperties(ISectionAccessoryProperties accessory, IEnumerable<TextDisplayProperties> components) : ComponentProperties, ISectionProperties, IEnumerable<TextDisplayProperties>
{
    public SectionProperties(ISectionAccessoryProperties accessory) : this(accessory, [])
    {
    }

    public override ComponentType ComponentType => ComponentType.Section;

    public IEnumerable<TextDisplayProperties> Components { get; set; } = components;

    public ISectionAccessoryProperties Accessory { get; set; } = accessory;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(TextDisplayProperties component) => AddComponents(component);

    IEnumerator<TextDisplayProperties> IEnumerable<TextDisplayProperties>.GetEnumerator() => Components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Components).GetEnumerator();
}

internal interface ISectionProperties
{
    [JsonPropertyName("type")]
    public abstract ComponentType ComponentType { get; }

    [JsonPropertyName("components")]
    public IEnumerable<TextDisplayProperties> Components { get; }

    [JsonPropertyName("accessory")]
    public ISectionAccessoryProperties Accessory { get; }
}

[JsonConverter(typeof(SectionAccessoryPropertiesConverter))]
public partial interface ISectionAccessoryProperties
{
    /// <summary>
    /// Type of the component.
    /// </summary>
    public ComponentType ComponentType { get; }

    public class SectionAccessoryPropertiesConverter : JsonConverter<ISectionAccessoryProperties>
    {
        public override ISectionAccessoryProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, ISectionAccessoryProperties value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case IButtonProperties buttonProperties:
                    JsonSerializer.Serialize(writer, buttonProperties, Serialization.Default.IButtonProperties);
                    break;
                case ThumbnailProperties thumbnailProperties:
                    JsonSerializer.Serialize(writer, thumbnailProperties, Serialization.Default.ThumbnailProperties);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid '{nameof(ISectionAccessoryProperties)}' value.");
            }
        }
    }
}
