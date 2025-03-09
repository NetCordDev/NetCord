using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

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
    public static ActionRowProperties Create(ReadOnlySpan<IButtonProperties> buttons) => new(buttons.ToArray());

    IEnumerator<IComponentProperties> IEnumerable<IComponentProperties>.GetEnumerator() => Components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Components).GetEnumerator();
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
