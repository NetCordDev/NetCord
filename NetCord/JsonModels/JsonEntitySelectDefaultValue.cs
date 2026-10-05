using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEntitySelectDefaultValue : JsonEntity
{
    [JsonPropertyName("type")]
    public JsonEntitySelectDefaultValueType Type { get; set; }
}
