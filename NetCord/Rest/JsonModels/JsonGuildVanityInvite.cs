using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonGuildVanityInvite
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("uses")]
    public required int Uses { get; set; }
}
