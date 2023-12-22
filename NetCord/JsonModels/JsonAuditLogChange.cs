using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAuditLogChange
{
    [JsonPropertyName("new_value")]
    public JsonElement? NewValue { get; set; }

    [JsonPropertyName("old_value")]
    public JsonElement? OldValue { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; }
}
