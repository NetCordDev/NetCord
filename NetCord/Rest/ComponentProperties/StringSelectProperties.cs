using System.Collections;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class StringSelectProperties(string customId, IEnumerable<StringSelectOptionProperties> options) : SelectProperties(customId), IStringSelectProperties, IEnumerable<StringSelectOptionProperties>
{
    public StringSelectProperties(string customId) : this(customId, [])
    {
    }

    public override ComponentType ComponentType => ComponentType.StringSelect;

    public IEnumerable<StringSelectOptionProperties> Options { get; set; } = options;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(StringSelectOptionProperties option) => AddOptions(option);

    private protected override void WriteToMessage(Utf8JsonWriter writer)
    {
        ActionRowProperties.WriteActionRowLike(writer, ParentId, this, Serialization.Default.IStringSelectProperties);
    }

    private protected override void WriteToLabel(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.IStringSelectProperties);
    }

    IEnumerator<StringSelectOptionProperties> IEnumerable<StringSelectOptionProperties>.GetEnumerator() => Options.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Options).GetEnumerator();
}

// Required not to serialize 'StringSelectProperties' as 'IEnumerable<out T>'
// https://github.com/dotnet/runtime/issues/63791
internal interface IStringSelectProperties : IInteractiveComponentProperties
{
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
    public IEnumerable<StringSelectOptionProperties> Options { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("required")]
    public bool? Required { get; set; }
}
