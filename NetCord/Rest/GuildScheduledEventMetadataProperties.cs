using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildScheduledEventMetadataProperties
{
    [JsonPropertyName("location")]
    public string Location { get; set; }

    public GuildScheduledEventMetadataProperties(string location)
    {
        Location = location;
    }
}
