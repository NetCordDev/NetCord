using System.Collections;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class StringMenuProperties(string customId, IEnumerable<StringMenuSelectOptionProperties> options) : MenuProperties(customId), IStringMenuProperties, IEnumerable<StringMenuSelectOptionProperties>
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

// Required not to serialize 'StringMenuProperties' as 'IEnumerable<out T>'
// https://github.com/dotnet/runtime/issues/63791
internal interface IStringMenuProperties
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType { get; }

    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    [JsonPropertyName("options")]
    public IEnumerable<StringMenuSelectOptionProperties> Options { get; set; }
}
