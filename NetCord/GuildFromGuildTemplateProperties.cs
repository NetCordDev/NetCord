using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildFromGuildTemplateProperties
{
    public GuildFromGuildTemplateProperties(string name)
    {
        Name = name;
    }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }
}
