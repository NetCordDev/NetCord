using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class PartyProperties
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("size")]
    public PartySizeProperties? Size { get; set; }

    [JsonSerializable(typeof(PartyProperties))]
    public partial class PartyPropertiesSerializerContext : JsonSerializerContext
    {
        public static PartyPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
