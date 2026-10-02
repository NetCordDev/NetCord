using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonApplicationRoleConnectionMetadata
{
    [JsonPropertyName("type")]
    public required ApplicationRoleConnectionMetadataType Type { get; set; }

    [JsonPropertyName("key")]
    public required string Key { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("name_localizations")]
    public IReadOnlyDictionary<string, string>? NameLocalizations { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("description_localizations")]
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations { get; set; }
}
