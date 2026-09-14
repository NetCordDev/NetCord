using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

/// <remarks>
/// This class is used when a role is returned in a context where not all properties are available, i.e. in <see cref="Rest.RestInvite"/>.
/// </remarks>
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
