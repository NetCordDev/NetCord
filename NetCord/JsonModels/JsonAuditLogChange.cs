using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonAuditLogChange
{
    [JsonPropertyName("new_value")]
    public JsonElement? NewValue { get; set; }

    [JsonPropertyName("old_value")]
    public JsonElement? OldValue { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonSerializable(typeof(JsonAuditLogChange))]
    public partial class JsonAuditLogChangeSerializerContext : JsonSerializerContext
    {
        public static JsonAuditLogChangeSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
