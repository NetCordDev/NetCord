using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonEntity
{
    [JsonPropertyName("id")]
    public required ulong Id { get; set; }
}
