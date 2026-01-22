using System.Collections;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class CheckboxGroupProperties(string customId, IEnumerable<CheckboxGroupOptionProperties> options) : ICheckboxGroupProperties, IEnumerable<CheckboxGroupOptionProperties>, IInteractiveComponentProperties, ILabelComponentProperties
{
    public CheckboxGroupProperties(string customId) : this(customId, [])
    {
    }

    public int? Id { get; set; }

    public ComponentType ComponentType => ComponentType.RadioGroup;

    public string CustomId { get; set; } = customId;

    public IEnumerable<CheckboxGroupOptionProperties> Options { get; set; } = options;

    public bool? Required { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(CheckboxGroupOptionProperties option) => AddOptions(option);

    void IJsonSerializable<ILabelComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.ICheckboxGroupProperties);
    }

    IEnumerator<CheckboxGroupOptionProperties> IEnumerable<CheckboxGroupOptionProperties>.GetEnumerator() => Options.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Options).GetEnumerator();
}

internal interface ICheckboxGroupProperties : IInteractiveComponentProperties
{
    [JsonPropertyName("options")]
    public IEnumerable<CheckboxGroupOptionProperties> Options { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("required")]
    public bool? Required { get; set; }
}

[GenerateMethodsForProperties]
public partial class CheckboxGroupOptionProperties(string label, string value)
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = label;

    [JsonPropertyName("value")]
    public string Value { get; set; } = value;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("default")]
    public bool IsDefault { get; set; }
}
