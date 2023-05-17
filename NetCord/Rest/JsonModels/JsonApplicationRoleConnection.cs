using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public partial class JsonApplicationRoleConnection
{
    [JsonPropertyName("platform_name")]
    public string? PlatformName { get; set; }

    [JsonPropertyName("platform_username")]
    public string? PlatformUsername { get; set; }

    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string> Metadata { get; set; }

    [JsonSerializable(typeof(JsonApplicationRoleConnection))]
    public partial class JsonApplicationRoleConnectionSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationRoleConnectionSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
