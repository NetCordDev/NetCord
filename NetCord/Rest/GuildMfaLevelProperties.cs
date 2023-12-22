using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class GuildMfaLevelProperties
{
    public GuildMfaLevelProperties(MfaLevel level)
    {
        Level = level;
    }

    [JsonPropertyName("level")]
    public MfaLevel Level { get; set; }
}
