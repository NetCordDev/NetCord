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

    [JsonSerializable(typeof(GuildScheduledEventMetadataProperties))]
    public partial class GuildScheduledEventMetadataPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildScheduledEventMetadataPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
