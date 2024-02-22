using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationRoleConnectionMetadataProperties(ApplicationRoleConnectionMetadataType type, string key, string name, string description)
{
    [JsonPropertyName("type")]
    public ApplicationRoleConnectionMetadataType Type { get; set; } = type;

    [JsonPropertyName("key")]
    public string Key { get; set; } = key;

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<string, string>? NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = description;

    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations { get; set; }
}
