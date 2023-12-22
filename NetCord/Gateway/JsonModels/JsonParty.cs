using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels;

public class JsonParty
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("size")]
    public int[]? Size { get; set; }
}
