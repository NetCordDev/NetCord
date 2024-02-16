using System.Text.Json.Serialization;

namespace NetCord.Rest;

internal class GuildMfaLevelProperties(MfaLevel level)
{
    [JsonPropertyName("level")]
    public MfaLevel Level { get; set; } = level;
}
