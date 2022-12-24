using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildScheduledEventMetadata
{
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonSerializable(typeof(JsonGuildScheduledEventMetadata))]
    public partial class JsonGuildScheduledEventMetadataSerializerContext : JsonSerializerContext
    {
        public static JsonGuildScheduledEventMetadataSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
