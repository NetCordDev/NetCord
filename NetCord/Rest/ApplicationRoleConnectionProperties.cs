using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationRoleConnectionProperties
{
    [JsonPropertyName("platform_name")]
    public string? PlatformName { get; set; }

    [JsonPropertyName("platform_username")]
    public string? PlatformUsername { get; set; }

    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; set; }

    [JsonSerializable(typeof(ApplicationRoleConnectionProperties))]
    public partial class ApplicationRoleConnectionPropertiesSerializerContext : JsonSerializerContext
    {
        public static ApplicationRoleConnectionPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
