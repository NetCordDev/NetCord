using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class LabelProperties(string label, ILabelComponentProperties component) : IModalComponentProperties
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Label;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; } = label;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("component")]
    public ILabelComponentProperties Component { get; set; } = component;

    void IJsonSerializable<IModalComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.LabelProperties);
    }
}
