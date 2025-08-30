using System.Collections;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class ComponentSectionProperties(IComponentSectionAccessoryComponentProperties accessory, IEnumerable<TextDisplayProperties> components) : IMessageComponentProperties, IComponentContainerComponentProperties, IComponentSectionProperties, IEnumerable<TextDisplayProperties>
{
    public ComponentSectionProperties(IComponentSectionAccessoryComponentProperties accessory) : this(accessory, [])
    {
    }

    public int? Id { get; set; }

    public ComponentType ComponentType => ComponentType.Section;

    public IEnumerable<TextDisplayProperties> Components { get; set; } = components;

    public IComponentSectionAccessoryComponentProperties Accessory { get; set; } = accessory;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(TextDisplayProperties component) => AddComponents(component);

    private void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.IComponentSectionProperties);
    }

    void IJsonSerializable<IMessageComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }

    void IJsonSerializable<IComponentContainerComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }

    IEnumerator<TextDisplayProperties> IEnumerable<TextDisplayProperties>.GetEnumerator() => Components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Components).GetEnumerator();
}

internal interface IComponentSectionProperties : IComponentProperties
{
    [JsonPropertyName("components")]
    public IEnumerable<TextDisplayProperties> Components { get; }

    [JsonPropertyName("accessory")]
    public IComponentSectionAccessoryComponentProperties Accessory { get; }
}
