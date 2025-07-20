using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

#pragma warning disable IDE0028 // Simplify collection initialization
#pragma warning disable IDE0306 // Simplify collection initialization

[CollectionBuilder(typeof(ComponentContainerProperties), nameof(Create))]
public partial class ComponentContainerProperties(IEnumerable<IComponentProperties> components) : IComponentProperties, IComponentContainerProperties, IEnumerable<IComponentProperties>
{
    public ComponentContainerProperties() : this([])
    {
    }

    public int? Id { get; set; }

    public ComponentType ComponentType => ComponentType.Container;

    public Color? AccentColor { get; set; }

    public bool Spoiler { get; set; }

    public IEnumerable<IComponentProperties> Components { get; set; } = components;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(IComponentProperties component) => AddComponents(component);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ComponentContainerProperties Create(ReadOnlySpan<IComponentProperties> components) => new(components.ToArray());

    IEnumerator<IComponentProperties> IEnumerable<IComponentProperties>.GetEnumerator() => Components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Components).GetEnumerator();

    public void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.IComponentContainerProperties);
    }
}

internal interface IComponentContainerProperties : IComponentProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("accent_color")]
    public Color? AccentColor { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spoiler")]
    public bool Spoiler { get; }

    [JsonPropertyName("components")]
    public IEnumerable<IComponentProperties> Components { get; }
}
