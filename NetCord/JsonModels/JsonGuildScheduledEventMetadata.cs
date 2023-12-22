using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonGuildScheduledEventMetadata
{
    [JsonPropertyName("location")]
    public string? Location { get; set; }
}
