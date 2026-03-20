using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class GuildScheduledEventMetadataProperties(string location)
{
    [JsonPropertyName("location")]
    public string Location { get; set; } = location;
}
