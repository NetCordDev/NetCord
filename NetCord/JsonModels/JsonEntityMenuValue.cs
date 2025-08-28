using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEntityMenuValue : JsonEntity
{
    [JsonPropertyName("type")]
    public JsonEntityMenuValueType Type { get; set; }
}
