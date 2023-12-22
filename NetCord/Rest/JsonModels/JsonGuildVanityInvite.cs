using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonGuildVanityInvite
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("uses")]
    public int Uses { get; set; }
}
