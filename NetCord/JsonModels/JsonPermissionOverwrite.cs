using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonPermissionOverwrite : JsonEntity
{
    [JsonPropertyName("type")]
    public PermissionOverwriteType Type { get; set; }

    [JsonPropertyName("allow")]
    public string Allowed { get; set; }

    [JsonPropertyName("deny")]
    public string Denied { get; set; }

    [JsonSerializable(typeof(JsonPermissionOverwrite))]
    public partial class JsonPermissionOverwriteSerializerContext : JsonSerializerContext
    {
        public static JsonPermissionOverwriteSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
