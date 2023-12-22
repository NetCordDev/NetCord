using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationRoleConnectionMetadataProperties
{
    public ApplicationRoleConnectionMetadataProperties(ApplicationRoleConnectionMetadataType type, string key, string name, string description)
    {
        Type = type;
        Key = key;
        Name = name;
        Description = description;
    }

    [JsonPropertyName("type")]
    public ApplicationRoleConnectionMetadataType Type { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<CultureInfo, string>? DescriptionLocalizations { get; set; }
}
