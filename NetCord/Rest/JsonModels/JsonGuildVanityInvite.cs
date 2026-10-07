using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.Rest.JsonModels;

[JsonGuard]
public partial class JsonGuildVanityInvite
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("uses")]
    public int Uses { get; set; }
}
