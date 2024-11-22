using System.Collections;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class StringMenuProperties(string customId, IEnumerable<StringMenuSelectOptionProperties> options) : MenuProperties(customId), IEnumerable<StringMenuSelectOptionProperties>
{
    public StringMenuProperties(string customId) : this(customId, [])
    {
    }

    public override ComponentType ComponentType => ComponentType.StringMenu;

    [JsonPropertyName("options")]
    public IEnumerable<StringMenuSelectOptionProperties> Options { get; set; } = options;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(StringMenuSelectOptionProperties option) => AddOptions(option);

    IEnumerator<StringMenuSelectOptionProperties> IEnumerable<StringMenuSelectOptionProperties>.GetEnumerator() => Options.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Options).GetEnumerator();
}
