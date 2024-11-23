using System.Collections;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ModalProperties(string customId, string title, IEnumerable<ComponentProperties> components) : IModalProperties, IEnumerable<ComponentProperties>
{
    public ModalProperties(string customId, string title) : this(customId, title, [])
    {
    }

    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; } = customId;

    [JsonPropertyName("title")]
    public string Title { get; set; } = title;

    [JsonPropertyName("components")]
    public IEnumerable<ComponentProperties> Components { get; set; } = components;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(ComponentProperties component) => AddComponents(component);

    IEnumerator<ComponentProperties> IEnumerable<ComponentProperties>.GetEnumerator() => Components.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Components).GetEnumerator();
}

// Required not to serialize 'ModalProperties' as 'IEnumerable<out T>'
// https://github.com/dotnet/runtime/issues/63791
internal interface IModalProperties
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("components")]
    public IEnumerable<ComponentProperties> Components { get; set; }
}
