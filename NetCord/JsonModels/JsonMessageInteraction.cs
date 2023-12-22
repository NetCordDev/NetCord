using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageInteraction : JsonEntity
{
    [JsonPropertyName("type")]
    public InteractionType Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }
}
