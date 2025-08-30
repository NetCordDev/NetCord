using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEntityMenuDefaultValue : JsonEntity
{
    [JsonPropertyName("type")]
    public JsonEntityMenuDefaultValueType Type { get; set; }
}
