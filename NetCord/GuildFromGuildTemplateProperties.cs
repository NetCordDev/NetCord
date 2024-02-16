using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildFromGuildTemplateProperties(string name)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }
}
