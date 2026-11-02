using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonGuildScheduledEventEntityMetadata
{
    [JsonPropertyName("location")]
    public string? Location { get; set; }
}
