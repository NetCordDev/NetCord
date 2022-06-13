using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonAuditLogChange
{
    [JsonPropertyName("new_value")]
    public JsonElement? NewValue { get; init; }

    [JsonPropertyName("old_value")]
    public JsonElement? OldValue { get; init; }

    [JsonPropertyName("key")]
    public string Key { get; init; }
}