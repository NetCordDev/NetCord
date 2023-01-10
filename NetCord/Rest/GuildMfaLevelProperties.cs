using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal partial class GuildMfaLevelProperties
{
    public GuildMfaLevelProperties(MfaLevel level)
    {
        Level = level;
    }

    [JsonPropertyName("level")]
    public MfaLevel Level { get; set; }

    [JsonSerializable(typeof(GuildMfaLevelProperties))]
    public partial class GuildMfaLevelPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildMfaLevelPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
