using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ComponentSeparatorProperties : IMessageComponentProperties, IComponentContainerComponentProperties
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Separator;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("divider")]
    public bool? Divider { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spacing")]
    public ComponentSeparatorSpacingSize? Spacing { get; set; }

    private void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.ComponentSeparatorProperties);
    }

    void IJsonSerializable<IMessageComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }

    void IJsonSerializable<IComponentContainerComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }
}
