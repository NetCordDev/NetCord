using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonSelectMenuDefaultValue : JsonEntity
{
    [JsonPropertyName("type")]
    public JsonSelectMenuDefaultValueType Type { get; set; }
}
