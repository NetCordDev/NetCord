using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal partial class JsonGuildMfaLevel
{
    [JsonPropertyName("level")]
    public MfaLevel Level { get; set; }

    [JsonSerializable(typeof(JsonGuildMfaLevel))]
    public partial class JsonGuildMfaLevelSerializerContext : JsonSerializerContext
    {
        public static JsonGuildMfaLevelSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
