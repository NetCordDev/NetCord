using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonAccount : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
