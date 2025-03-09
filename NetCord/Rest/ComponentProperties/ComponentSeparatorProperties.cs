using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ComponentSeparatorProperties : IComponentProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Separator;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("divider")]
    public bool? Divider { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spacing")]
    public ComponentSeparatorSpacingSize? Spacing { get; set; }
}
