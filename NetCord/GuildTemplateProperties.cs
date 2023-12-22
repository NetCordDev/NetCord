using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildTemplateProperties
{
    public GuildTemplateProperties(string name)
    {
        Name = name;
    }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
