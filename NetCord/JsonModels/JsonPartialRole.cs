using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonPartialRole : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("colors")]
    public JsonRoleColors Colors { get; set; }

    [JsonPropertyName("icon")]
    public string? IconHash { get; set; }

    [JsonPropertyName("unicode_emoji")]
    public string? UnicodeEmoji { get; set; }
}
