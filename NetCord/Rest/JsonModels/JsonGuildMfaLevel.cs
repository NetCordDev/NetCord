using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

internal class JsonGuildMfaLevel
{
    [JsonPropertyName("level")]
    public MfaLevel Level { get; set; }
}
