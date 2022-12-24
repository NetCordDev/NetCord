using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class StageInstanceOptions
{
    internal StageInstanceOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("privacy_level")]
    public StageInstancePrivacyLevel? PrivacyLevel { get; set; }

    [JsonSerializable(typeof(StageInstanceOptions))]
    public partial class StageInstanceOptionsSerializerContext : JsonSerializerContext
    {
        public static StageInstanceOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
