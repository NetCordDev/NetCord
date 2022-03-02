using System.Text.Json.Serialization;

namespace NetCord;

public class PartyProperties
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("size")]
    public PartySizeProperties? Size { get; }
}