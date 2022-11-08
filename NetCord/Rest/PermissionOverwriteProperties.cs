using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class PermissionOverwriteProperties
{
    [JsonPropertyName("id")]
    public ulong Id { get; }

    [JsonPropertyName("type")]
    public PermissionOverwriteType Type { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("allow")]
    public Permission? Allowed { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("deny")]
    public Permission? Denied { get; set; }

    public PermissionOverwriteProperties(ulong id, PermissionOverwriteType type)
    {
        Id = id;
        Type = type;
    }

    [JsonSerializable(typeof(PermissionOverwriteProperties))]
    public partial class PermissionOverwritePropertiesSerializerContext : JsonSerializerContext
    {
        public static PermissionOverwritePropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
