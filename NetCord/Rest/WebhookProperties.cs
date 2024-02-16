using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class WebhookProperties(string name)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar")]
    public ImageProperties? Avatar { get; set; }
}
