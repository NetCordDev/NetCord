using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEntitySelectDefaultValue : JsonEntity
{
    [JsonPropertyName("type")]
    public JsonEntityMenuDefaultValueType Type { get; set; }
}
