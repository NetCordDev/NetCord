using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonPermissionOverwrite : JsonEntity
{
    [JsonPropertyName("type")]
    public PermissionOverwriteType Type { get; set; }

    [JsonPropertyName("allow")]
    public Permissions Allowed { get; set; }

    [JsonPropertyName("deny")]
    public Permissions Denied { get; set; }
}
