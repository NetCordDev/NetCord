using System.Text.Json.Serialization;

namespace NetCord.Gateway;

[GenerateMethodsForProperties]
public partial class PartyProperties
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("size")]
    public PartySizeProperties? Size { get; set; }
}
