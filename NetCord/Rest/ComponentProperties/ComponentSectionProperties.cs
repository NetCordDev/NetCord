using System.Collections;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ComponentSectionProperties(IComponentSectionAccessoryProperties accessory, IEnumerable<TextDisplayProperties> components) : IComponentProperties, IComponentSectionProperties, IEnumerable<TextDisplayProperties>
{
    public ComponentSectionProperties(IComponentSectionAccessoryProperties accessory) : this(accessory, [])
    {
    }

    public int? Id { get; set; }

    public ComponentType ComponentType => ComponentType.Section;

    public IEnumerable<TextDisplayProperties> Components { get; set; } = components;

    public IComponentSectionAccessoryProperties Accessory { get; set; } = accessory;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(TextDisplayProperties component) => AddComponents(component);

    IEnumerator<TextDisplayProperties> IEnumerable<TextDisplayProperties>.GetEnumerator() => Components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Components).GetEnumerator();
}

internal interface IComponentSectionProperties : IComponentProperties
{
    [JsonPropertyName("components")]
    public IEnumerable<TextDisplayProperties> Components { get; }

    [JsonPropertyName("accessory")]
    public IComponentSectionAccessoryProperties Accessory { get; }
}
