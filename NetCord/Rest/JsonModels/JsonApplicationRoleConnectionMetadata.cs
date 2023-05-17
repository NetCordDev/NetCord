using System.Globalization;
using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public partial class JsonApplicationRoleConnectionMetadata
{
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

    [JsonSerializable(typeof(JsonApplicationRoleConnectionMetadata))]
    public partial class JsonApplicationRoleConnectionMetadataSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationRoleConnectionMetadataSerializerContext WithOptions { get; } = new(Serialization.Options);
    }

    [JsonSerializable(typeof(IEnumerable<JsonApplicationRoleConnectionMetadata>))]
    public partial class IEnumerableOfJsonApplicationRoleConnectionMetadataSerializerContext : JsonSerializerContext
    {
        public static IEnumerableOfJsonApplicationRoleConnectionMetadataSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
