using System.Collections;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class ComponentSectionProperties(IComponentSectionAccessoryComponentProperties accessory, IEnumerable<IComponentSectionComponentProperties> components) : IMessageComponentProperties, IComponentContainerComponentProperties, IComponentSectionProperties, IEnumerable<IComponentSectionComponentProperties>
{
    public ComponentSectionProperties(IComponentSectionAccessoryComponentProperties accessory) : this(accessory, [])
    {
    }

    public int? Id { get; set; }

    public ComponentType ComponentType => ComponentType.Section;

    public IEnumerable<IComponentSectionComponentProperties> Components { get; set; } = components;

    public IComponentSectionAccessoryComponentProperties Accessory { get; set; } = accessory;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(IComponentSectionComponentProperties component) => AddComponents(component);

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

    IEnumerator<IComponentSectionComponentProperties> IEnumerable<IComponentSectionComponentProperties>.GetEnumerator() => Components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Components).GetEnumerator();
}

internal interface IComponentSectionProperties : IComponentProperties
{
    [JsonPropertyName("components")]
    public IEnumerable<IComponentSectionComponentProperties> Components { get; }

    [JsonPropertyName("accessory")]
    public IComponentSectionAccessoryComponentProperties Accessory { get; }
}
