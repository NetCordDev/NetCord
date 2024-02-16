using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class PermissionOverwriteProperties(ulong id, PermissionOverwriteType type)
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; } = id;

    [JsonPropertyName("type")]
    public PermissionOverwriteType Type { get; set; } = type;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("allow")]
    public Permissions? Allowed { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("deny")]
    public Permissions? Denied { get; set; }
}
