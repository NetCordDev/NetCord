using System.Text.Json.Serialization;

namespace NetCord;

public class GuildFromGuildTemplateProperties
{
    public GuildFromGuildTemplateProperties(string name)
    {
        Name = name;
    }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }
}